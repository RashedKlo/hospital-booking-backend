using System;

namespace hospital_booking.Data.DTOs.Admin
{
    public class AdminDto
    {
        public int AdminId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
