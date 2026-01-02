using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Doctor
{
    public class DoctorAddDto
    {
        [Required(ErrorMessage = "Clinic ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Clinic ID")]
        public int ClinicId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 250 characters")]
        public string FullName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
        public string? Phone { get; set; }

        [Range(0, 70, ErrorMessage = "Experience years must be between 0 and 70")]
        public int ExperienceYears { get; set; }
    }
}

