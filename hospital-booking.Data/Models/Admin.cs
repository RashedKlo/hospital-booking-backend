using System;

namespace hospital_booking.Data.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "admin"; // super_admin, admin, receptionist
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AdminAuthenticationData
    {
        public Admin Admin { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public AdminAuthenticationData(Admin admin, string accessToken, string refreshToken)
        {
            Admin = admin;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}