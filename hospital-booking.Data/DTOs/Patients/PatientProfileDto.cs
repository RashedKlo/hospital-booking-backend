using System;

namespace hospital_booking.Data.DTOs.Patient
{
    public class PatientProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsGoogleLogin { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; }
        public int SuspensionCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}