using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hospital_booking.Data.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsGoogleLogin { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; }
        public int SuspensionCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class PatientAuthenticationData
    {
        public Patient Patient { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public PatientAuthenticationData(Patient patient, string accessToken, string refreshToken)
        {
            Patient = patient;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }

}