using System;
using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Admin
{
    public class AdminAddDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 250 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(250, ErrorMessage = "Email cannot exceed 250 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [StringLength(100, ErrorMessage = "Role cannot exceed 100 characters")]
        public string Role { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
        public string Phone { get; set; } = string.Empty;
    }
}

