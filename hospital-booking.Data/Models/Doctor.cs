using System;

namespace hospital_booking.Data.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int SpecialtyId { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? Rating { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class DoctorAuthenticationData
    {
        public Doctor Doctor { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public DoctorAuthenticationData(Doctor doctor, string accessToken, string refreshToken)
        {
            Doctor = doctor;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}