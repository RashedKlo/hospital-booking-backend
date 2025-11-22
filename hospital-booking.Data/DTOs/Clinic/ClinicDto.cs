using System;

namespace hospital_booking.Data.DTOs.Clinic
{
    public class ClinicDto
    {
        public int ClinicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
