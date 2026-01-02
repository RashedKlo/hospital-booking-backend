using System;

namespace hospital_booking.Data.DTOs.ClinicFacility
{
    public class ClinicFacilityDto
    {
        public int FacilityId { get; set; }
        public int ClinicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
