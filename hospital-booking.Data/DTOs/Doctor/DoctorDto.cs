using hospital_booking.Data.DTOs.Clinic;

namespace hospital_booking.Data.DTOs.Doctor
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public int ExperienceYears { get; set; }
        
        // Nested object
        public ClinicDto? Clinic { get; set; }
    }
}
