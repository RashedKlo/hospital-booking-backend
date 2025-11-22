using System;

namespace hospital_booking.Data.DTOs.Doctor
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int ExperienceYears { get; set; } = 0;
    }
}
