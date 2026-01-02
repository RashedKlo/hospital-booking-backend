using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Admin
{
    public class AdminUpdateDto
    {
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 250 characters")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(250, ErrorMessage = "Email cannot exceed 250 characters")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Role cannot exceed 100 characters")]
        public string? Role { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
        public string? Phone { get; set; }

        public bool? IsActive { get; set; }
    }
}
