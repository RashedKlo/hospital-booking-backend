using System;

    namespace hospital_booking.Data.DTOs.Clinic
{
    public class ClinicDto
    {
        public int ClinicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? ReviewCount { get; set; }
        public string? OpeningHours { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
     }
}
