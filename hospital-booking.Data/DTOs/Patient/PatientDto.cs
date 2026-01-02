using System;
using hospital_booking.Data.DTOs.User;

namespace hospital_booking.Data.DTOs.Patient
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Notes { get; set; }

        public UserDto? User { get; set; }
    }
}
