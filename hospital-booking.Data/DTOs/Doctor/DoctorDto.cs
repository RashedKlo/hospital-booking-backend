using System;

namespace hospital_booking.Data.DTOs.Doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int SpecialtyId { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? Rating { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}