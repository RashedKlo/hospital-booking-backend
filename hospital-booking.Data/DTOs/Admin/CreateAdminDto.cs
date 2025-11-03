using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Admin
{
    public class CreateAdminDto
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
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(super_admin|admin|receptionist)$", ErrorMessage = "Role must be super_admin, admin, or receptionist")]
        public string Role { get; set; } = "admin";
    }
}