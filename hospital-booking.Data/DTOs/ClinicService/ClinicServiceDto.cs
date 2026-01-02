using System;

namespace hospital_booking.Data.DTOs.ClinicService
{
    public class ClinicServiceDto
    {
        public int ServiceId { get; set; }
        public int ClinicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
