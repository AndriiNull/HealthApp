using HealthApp.Server.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.Server.Models.DTOs
{
    public class AppointmentCreateDto
    {
        public int IssueId { get; set; }
        public int DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "Scheduled";
    }

    public class AppointmentReadDto
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public int DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }

        public List<AppointmentComment>? Comments { get; set; }
    }

    public class AppointmentUpdateDto
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
    }
    public class AppointmentCommentDto
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class AddAppointmentCommentDto
    {
        public string CommentText { get; set; } = string.Empty;
    }
    public class AppointmentSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    public class AppointmentBook
    {
        public int DoctorID { get; set; }
        public int PatientId { get; set; }
        public int IssueId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time format.")]
        [StartTimeValidation] // ✅ Custom validation (defined below)
        public DateTime StartTime { get; set; }
    }
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int? IssueId { get; set; }
    }
    public class StartTimeValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is DateTime startTime)
                {
                    // ✅ Ensure StartTime is not the default DateTime value
                    if (startTime == default)
                    {
                        return new ValidationResult("Start time must be a valid date and time.");
                    }

                    // ✅ Ensure StartTime includes both Date and Time
                    if (startTime.TimeOfDay == TimeSpan.Zero)
                    {
                        return new ValidationResult("Start time must include both date and time.");
                    }

                    return ValidationResult.Success;
                }

                return new ValidationResult("Invalid date format.");
            }
        }
    }
   

