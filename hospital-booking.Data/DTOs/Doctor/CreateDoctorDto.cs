using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Doctor
{
    public class CreateDoctorDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 255 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [RegularExpression(@"^[\d\s\+\-\(\)]+$", ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specialty is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Specialty ID must be a positive number")]
        public int SpecialtyId { get; set; }

        [Range(0, 70, ErrorMessage = "Experience years must be between 0 and 70")]
        public int? ExperienceYears { get; set; }

        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
        public string? Bio { get; set; }
    }
}