using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Admin
{
    public class UpdateAdminDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 255 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(super_admin|admin|receptionist)$", ErrorMessage = "Role must be super_admin, admin, or receptionist")]
        public string Role { get; set; } = "admin";
    }
}