using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HealthApp.Server.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HealthApp.Server.Models;

public partial class MasterContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public MasterContext(DbContextOptions<MasterContext> options) : base(options) { }

    public DbSet<Person> People { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<Diagnosis> Diagnoses { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Analysis> Analyses { get; set; }
    public DbSet<AnalysisConclusion> AnalysisConclusions { get; set; }
    public DbSet<Conclusion> Conclusions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Licence> Licences { get; set; }
    public DbSet<DoctorsLicences> DoctorsLicences { get; set; }
    public DbSet<AppointmentComment> AppointmentComments { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ApplicationRole> ApplicationRoles { get; set; }
    public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        ConfigureAutoIncrement(modelBuilder);
        ConfigureRelationships (modelBuilder);
        DoSeeding(modelBuilder);

      
    }
    
    private void ConfigureAutoIncrement(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Doctor>().Property(d => d.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<DoctorsLicences>().Property(dl => dl.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Licence>().Property(l => l.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Practices>().Property(p => p.Id).ValueGeneratedOnAdd();
        //modelBuilder.Entity<Patient>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Issue>().Property(i => i.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Appointment>().Property(a => a.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Diagnosis>().Property(d => d.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Prescription>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Category>().Property(c => c.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Conclusion>().Property(c => c.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Analysis>().Property(a => a.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<AnalysisConclusion>().Property(ac => ac.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<AppointmentComment>().Property(ac => ac.Id).ValueGeneratedOnAdd();
        // 🔹 Ensure Role Primary Keys (Explicitly Set - No Auto Increment)
        modelBuilder.Entity<ApplicationRole>().Property(r => r.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Recommendation>()
           .Property(r => r.Id)
           .ValueGeneratedOnAdd();

        // 🔹 Ensure ApplicationUser Uses Guid for Id (No Auto Increment Needed)
        modelBuilder.Entity<ApplicationUser>().Property(u => u.Id).ValueGeneratedNever();
    }
    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Ensure One-to-One Relation: Person ⇄ ApplicationUser
        modelBuilder.Entity<Person>()
            .HasOne(p => p.User)
            .WithOne(u => u.Person)
            .HasForeignKey<ApplicationUser>(u => u.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Other Relations
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Person)
            .WithOne()
            .HasForeignKey<Doctor>(d => d.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.PersonNavigation)
            .WithOne()
            .HasForeignKey<Patient>(p => p.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DoctorsLicences>()
            .HasOne(dl => dl.Doctor)
            .WithMany(d => d.DoctorsLicences)
            .HasForeignKey(dl => dl.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DoctorsLicences>()
            .HasOne(dl => dl.Licence)
            .WithMany(l => l.DoctorsLicences)
            .HasForeignKey(dl => dl.LicenceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Issue>()
            .HasOne(i => i.PatientNavigation)
            .WithMany()
            .HasForeignKey(i => i.Patient)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Issue>()
            .HasOne(i => i.LeadDoctorNavigation)
            .WithMany()
            .HasForeignKey(i => i.LeadDoctor)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Licence>()
            .HasOne(l => l.Practices)
            .WithMany(p => p.Licences)
            .HasForeignKey(l => l.PracticesId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppointmentComment>()
            .HasOne(ac => ac.Appointment)
            .WithMany(a => a.Comments)
            .HasForeignKey(ac => ac.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Diagnosis>()
            .HasOne(d => d.IssueNavigation)
            .WithMany()
            .HasForeignKey(d => d.Issue)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Diagnosis>()
            .HasOne(d => d.DoctorNavigation)
            .WithMany()
            .HasForeignKey(d => d.Doctor)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.IssueNavigation)
            .WithMany()
            .HasForeignKey(a => a.Issue)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.DoctorNavigation)
            .WithMany()
            .HasForeignKey(a => a.Doctor)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Analysis>()
            .HasOne(a => a.IssueNavigation)
            .WithMany()
            .HasForeignKey(a => a.Issue)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Analysis>()
            .HasOne(a => a.CategoryNavigation)
            .WithMany()
            .HasForeignKey(a => a.Category)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnalysisConclusion>()
            .HasOne(ac => ac.AnalysisNavigation)
            .WithMany(a => a.AnalysisConclusions)
            .HasForeignKey(ac => ac.Analysis)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnalysisConclusion>()
            .HasOne(ac => ac.ConclusionNavigation)
            .WithMany(c => c.AnalysisConclusions)
            .HasForeignKey(ac => ac.Conclusion)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnalysisConclusion>()
            .HasOne(ac => ac.DoctorNavigation)
            .WithMany()
            .HasForeignKey(ac => ac.Doctor)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.DiagnosisNavigation)
            .WithMany()
            .HasForeignKey(p => p.Diagnosis)
            .OnDelete(DeleteBehavior.Cascade);
       modelBuilder.Entity<Recommendation>()
            .HasOne(r => r.Issue)
            .WithMany(i => i.Recommendations )
            .HasForeignKey(r => r.IssueId)
            .OnDelete(DeleteBehavior.Cascade); // 🔥 Deletes recommendations if issue is deleted

        modelBuilder.Entity<Recommendation>()
            .HasOne(r => r.Doctor)
            .WithMany()
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

    }

    private void DoSeeding(ModelBuilder modelBuilder)
    {
        AddPracticesSeeding(modelBuilder);
        AddCategoriesSeeding(modelBuilder);
        AddPeopleSeeding(modelBuilder);
        AddPatientsSeeding(modelBuilder);
        AddDoctorsSeeding(modelBuilder);
        AddLicensesSeeding(modelBuilder);
        AddDoctorsLicensesSeeding(modelBuilder);
        AddIssuesSeeding(modelBuilder);
        AddAppointmentsSeeding(modelBuilder);
        AddDiagnosesSeeding(modelBuilder);
        AddPrescriptionsSeeding(modelBuilder);
        AddConclusionsSeeding(modelBuilder);
        AddAnalysisSeeding(modelBuilder);
        AddAnalysisConclusionsSeeding(modelBuilder);
        AddAppointmentCommentsSeeding(modelBuilder);
    }
    private void AddPracticesSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Practices>().HasData(
            new Practices { Id = 1, Practice = "General Medicine" },
            new Practices { Id = 2, Practice = "Pediatrics" },
            new Practices { Id = 3, Practice = "Cardiology" },
            new Practices { Id = 4, Practice = "Neurology" },
            new Practices { Id = 5, Practice = "Orthopedics" },
            new Practices { Id = 6, Practice = "Dermatology" },
            new Practices { Id = 7, Practice = "Psychiatry" }
        );
    }

    private void AddCategoriesSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Infection", ReferenceValue = "Bacterial/Viral", Global = true },
            new Category { Id = 2, Name = "Chronic", ReferenceValue = "Diabetes, Hypertension", Global = true },
            new Category { Id = 3, Name = "Acute", ReferenceValue = "Surgery Required", Global = false }
        );
    }
   
    private void AddPeopleSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasData(
         new Person { Id = 1, Name = "John", Surname = "Doe", Gender = "Male", Birthday = new DateTime(1990, 1, 1) },
         new Person { Id = 2, Name = "Jane", Surname = "Smith", Gender = "Female", Birthday = new DateTime(1992, 5, 10) });



       //  ✅ 2️⃣ Seed Users That Reference People
        //modelBuilder.Entity<ApplicationUser>().HasData(
        //     new ApplicationUser
        //     {
        //         Id = "1", // ✅ Use a GUID instead of "1"
        //         PersonId = 1,
        //         Email = "admin@example.com",
        //         UserName = "admin@example.com",
        //         NormalizedEmail = "ADMIN@EXAMPLE.COM",
        //         NormalizedUserName = "ADMIN@EXAMPLE.COM",
        //         EmailConfirmed = true
        //     }

        //);

      //   ✅ 3️⃣ Seed Roles
        //modelBuilder.Entity<ApplicationRole>().HasData(
        //   new ApplicationRole { Id = Guid.Parse("1").ToString(), Name = "Doctor", NormalizedName = "DOCTOR" },
        //new ApplicationRole { Id = Guid.Parse("2").ToString(), Name = "Patient", NormalizedName = "PATIENT" }
        //);

        //// ✅ 4️⃣ Assign User to Role
        //modelBuilder.Entity<ApplicationUserRole>().HasData(
        //    new ApplicationUserRole { UserId = Guid.Parse("1").ToString(), RoleId = AdminRoleId }
        //);
    }

    private void AddPatientsSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasData(
            new Patient {  PersonId = 1 },
            new Patient {  PersonId = 2 }
        );
    }

    private void AddDoctorsSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { Id = 1, PersonId = 1 },
            new Doctor { Id = 2, PersonId = 2 }
        );
    }

    private void AddLicensesSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Licence>().HasData(
            new Licence { Id = 1, IssueDate = new DateTime(2019, 1, 1), ExpirationDate = new DateTime(2029, 1, 1), PracticesId = 1 },
            new Licence { Id = 2, IssueDate = new DateTime(2021, 5, 15), ExpirationDate = new DateTime(2031, 5, 15), PracticesId = 2 }
        );
    }

    private void AddDoctorsLicensesSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoctorsLicences>().HasData(
            new DoctorsLicences { Id = 1, LicenceId = 1, DoctorId = 1 },
            new DoctorsLicences { Id = 2, LicenceId = 2, DoctorId = 2 }
        );
    }

    private void AddIssuesSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Issue>().HasData(
        new Issue { Id = 1, Patient = 1, LeadDoctor = 1, CreatedAt = new DateTime(2024, 2, 10), Status = "Open" },
        new Issue { Id = 2, Patient = 2, LeadDoctor = 2, CreatedAt = new DateTime(2024, 2, 15), Status = "In Progress" }
        );
    }

    private void AddAppointmentsSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>().HasData(
            new Appointment { Id = 1, Issue = 1, Doctor = 1, StartTime = new DateTime(2024, 1, 10, 9, 0, 0), EndTime = new DateTime(2024, 1, 10, 10, 0, 0), Status = "Completed" },
            new Appointment { Id = 2, Issue = 2, Doctor = 2, StartTime = new DateTime(2024, 2, 15, 14, 0, 0), EndTime = new DateTime(2024, 2, 15, 15, 0, 0), Status = "Scheduled" }
        );
    }

    private void AddDiagnosesSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Diagnosis>().HasData(
            new Diagnosis { Id = 1, Issue = 1, Doctor = 1 },
            new Diagnosis { Id = 2, Issue = 2, Doctor = 2 }
        );
    }

    private void AddPrescriptionsSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prescription>().HasData(
            new Prescription { Id = 1, Diagnosis = 1, Medicine = "Antibiotics", CreatedDate = new DateTime(2024, 1, 15) },
            new Prescription { Id = 2, Diagnosis = 2, Medicine = "Painkillers", CreatedDate = new DateTime(2024, 2, 18) }
        );
    }

    private void AddConclusionsSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conclusion>().HasData(
            new Conclusion { Id = 1, IssueDate = new DateTime(2023, 11, 1) },
            new Conclusion { Id = 2, IssueDate = new DateTime(2023, 12, 5) }
        );
    }

    private void AddAnalysisSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Analysis>().HasData(
            new Analysis { Id = 1, Issue = 1, Category = 1, Value = "100" },
            new Analysis { Id = 2, Issue = 2, Category = 2, Value = "200" }
        );
    }

    private void AddAnalysisConclusionsSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalysisConclusion>().HasData(
            new AnalysisConclusion { Id = 1, Analysis = 1, Conclusion = 1, Doctor = 1 },
            new AnalysisConclusion { Id = 2, Analysis = 2, Conclusion = 2, Doctor = 2 }
        );
    }
    private void AddAppointmentCommentsSeeding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentComment>().HasData(
               new AppointmentComment { Id = 1, AppointmentId = 1, CommentText = "Patient is responding well to treatment.", CreatedAt = new DateTime(2024, 2, 1, 10, 0, 0) },
               new AppointmentComment { Id = 2, AppointmentId = 2, CommentText = "Follow-up required in 2 weeks.", CreatedAt = new DateTime(2024, 2, 1, 11, 0, 0) },
               new AppointmentComment { Id = 3, AppointmentId = 2, CommentText = "Blood test results pending.", CreatedAt = new DateTime(2024, 2, 2, 9, 30, 0) }
        );
    }
}

    