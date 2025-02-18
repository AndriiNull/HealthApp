using HealthApp.Server.Models;
using HealthApp.Server.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly MasterContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, MasterContext masterContext, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = masterContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized(new { error = "Invalid credentials" });

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user);
            
            return Ok(new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    roles
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterRequest request)
        {
            // ✅ Step 1: Check if email already exists
            if (await _context.ApplicationUsers.AnyAsync(e => e.Email == request.Email))
            {
                return BadRequest(new { error = "Email already exists" });
            }

            // ✅ Step 2: Create and Save Person first
            var person = new Person
            {
                Name = request.Name,
                Birthday = request.Birthdate,
                Gender = "some",
                Surname = request.Surname
            };

            await _context.People.AddAsync(person);
            await _context.SaveChangesAsync(); // Ensures PersonId is generated

            // ✅ Step 3: Create ApplicationUser
            var user = new ApplicationUser
            {
                PersonId = person.Id,
                Email = request.Email,
                UserName = request.Email

            };
           
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { error = "Failed to create user", details = result.Errors });
            }

            // ✅ Step 4: Ensure "Patient" Role Exists
            if (!await _roleManager.RoleExistsAsync("Patient"))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Patient" });
            }
            if (!await _roleManager.RoleExistsAsync("Doctor"))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Doctor" });
            }
            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Manager" });
            }

            // ✅ Step 5: Assign User to "Patient" Role
            await _userManager.AddToRoleAsync(user, "Patient");

            await _context.AddAsync(new Patient() { PersonId = person.Id });

            await _context.SaveChangesAsync();
            // ✅ Step 6: Generate JWT Token for immediate authentication
            var token = GenerateJwtToken(user);

            Console.WriteLine($"✅ New Patient Registered. Email: {user.Email}, PersonId: {user.PersonId}");
            Console.WriteLine($"✅ Generated JWT: {token}");

            return Ok(new
            {
                message = "User registered as Patient successfully.",
                token, // ✅ Return JWT Token
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    role = "Patient"
                }
            });
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var secretKey = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("❌ ERROR: JWT Secret must be at least 32 characters long!");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expirationTime = DateTime.UtcNow.AddHours(12);
            var issuedAt = DateTime.UtcNow;

            var claims = new List<Claim>
    {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("PatientId", user.PersonId.ToString()),
                new Claim("user_email", user.Email),
                new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)issuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
    };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // 🔥 Store multiple roles
            }
            
            Console.WriteLine("🔍 Final Claims Before Token Generation:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"🔹 {claim.Type}: {claim.Value}");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                IssuedAt = issuedAt,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.InboundClaimTypeMap.Clear(); // ✅ Prevents unwanted claim filtering

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            Console.WriteLine($"✅ Final Generated JWT: {jwt}");

            // 🔍 Debug: Verify Base64 encoding of JWT parts
            var parts = jwt.Split('.');
            if (parts.Length != 3)
            {
                throw new InvalidOperationException("❌ ERROR: Generated JWT does not have 3 parts (header.payload.signature)!");
            }

            Console.WriteLine($"🔍 JWT Header (Base64Url Encoded): {parts[0]}");
            Console.WriteLine($"🔍 JWT Payload (Base64Url Encoded): {parts[1]}");

            // Decode Base64Url manually for debugging
            try
            {
                string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
                Console.WriteLine($"🔍 Decoded JWT Header JSON: {headerJson}");

                string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                Console.WriteLine($"🔍 Decoded JWT Payload JSON: {payloadJson}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR decoding JWT parts: {ex.Message}");
            }

            return jwt;
        }

        // ✅ Helper function to decode Base64Url manually
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
    public class RegisterRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress] // ✅ Prevents invalid email formats
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] // ✅ Helps enforce secure input
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
    }
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
