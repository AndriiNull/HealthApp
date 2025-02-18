using HealthApp.Server.Models;
using HealthApp.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;
IdentityModelEventSource.LogCompleteSecurityArtifact = true;
LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .AddFilter("Microsoft.IdentityModel", LogLevel.Debug) // ✅ Logs detailed security artifacts
        .AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
}); 
Environment.SetEnvironmentVariable("DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION", "1");
Environment.SetEnvironmentVariable("DOTNET_SYSTEM_CONSOLE_ALLOW_SECURITY_ARTIFACTS", "1");

// ✅ Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<LicenceService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<IssueService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();
builder.Services.AddScoped<IRoleService, RoleService>();
// ✅ Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ✅ Configure Database Connection
builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenHandlers.Clear(); // Remove default handler
            options.TokenHandlers.Add(new CustomJwtSecurityTokenHandler());
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse(); // Prevents default redirect
                    context.Response.StatusCode = 401; // Return 401 Unauthorized instead
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Unauthorized access\"}");
                },
                OnMessageReceived = context =>
                {
                    if (context.Token != null)
                    {
                        Console.WriteLine($"📢 Received JWT: {context.Token}");

                        // Decode JWT header for debugging
                        try
                        {
                            string[] parts = context.Token.Split('.');
                            if (parts.Length == 3)
                            {
                                string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
                                Console.WriteLine($"🔍 Decoded Received JWT Header: {headerJson}");
                            }
                            else
                            {
                                Console.WriteLine("❌ Invalid JWT structure (must have 3 parts).");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error decoding received JWT header: {ex.Message}");
                        }
                    }
                    Console.WriteLine("Successfull auth");
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"❌ JWT Authentication Failed: {context.Exception.Message}");

                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (authHeader != null && authHeader.StartsWith("Bearer "))
                    {
                        var token = authHeader.Substring("Bearer ".Length).Trim();
                        Console.WriteLine($"🔍 Extracted JWT Token: {token}");

                        try
                        {
                            string[] parts = token.Split('.');
                            if (parts.Length == 3)
                            {
                                // ✅ Manually decode JWT header
                                string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
                                Console.WriteLine($"🔍 Decoded JWT Header JSON: {headerJson}");

                                // ✅ Decode JWT payload
                                string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                                Console.WriteLine($"🔍 Decoded JWT Payload JSON: {payloadJson}");

                                // ✅ Try parsing claims manually
                                var jwtHandler = new JwtSecurityTokenHandler();
                                if (jwtHandler.CanReadToken(token))
                                {
                                    var jwtToken = jwtHandler.ReadJwtToken(token);
                                    Console.WriteLine("✅ JWT Token Structure is valid.");
                                    Console.WriteLine("🔍 Token Claims:");

                                    foreach (var claim in jwtToken.Claims)
                                    {
                                        Console.WriteLine($"🔹 {claim.Type}: {claim.Value}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("❌ JWT Token is malformed (Cannot be read). Possible cause of IDX14100.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("❌ Invalid JWT structure (must have 3 parts: header.payload.signature).");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ ERROR decoding JWT: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ No JWT Token found in Authorization Header.");
                    }

                    return Task.CompletedTask;

                }
            };
        });


// ✅ Configure Identity (Users & Roles)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<MasterContext>()
    .AddDefaultTokenProviders();
// ✅ Configure Authentication (JWT)



// ✅ Add Swagger for API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Serve Static Files for SPA
app.UseDefaultFiles();
app.UseStaticFiles();

// ✅ Ensure CORS is applied before Authentication & Authorization
app.UseCors("AllowAllOrigins");

// ✅ Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
byte[] Base64UrlDecode(string base64Url)
{
    string base64 = base64Url.Replace('-', '+').Replace('_', '/');
    switch (base64.Length % 4)
    {
        case 2: base64 += "=="; break;
        case 3: base64 += "="; break;
    }
    return Convert.FromBase64String(base64);
}
public class CustomJwtSecurityTokenHandler : JwtSecurityTokenHandler
{
    public override bool CanReadToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        // ✅ Ensure the token has 3 parts (Header.Payload.Signature)
        string[] parts = token.Split('.');
        return parts.Length == 3;
    }

    public override SecurityToken ReadToken(string token)
    {
        Console.WriteLine($"📢 Custom Decoder - Received JWT: {token}");

        try
        {
            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new Exception("❌ Invalid JWT structure (must have 3 parts: Header.Payload.Signature).");
            }

            // ✅ Manually decode Base64Url header and payload
            string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));

            Console.WriteLine($"🔍 Decoded JWT Header: {headerJson}");
            Console.WriteLine($"🔍 Decoded JWT Payload: {payloadJson}");

            return base.ReadToken(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Custom Decoder Error: {ex.Message}");
            throw;
        }
    }

    private static byte[] Base64UrlDecode(string base64Url)
    {
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}