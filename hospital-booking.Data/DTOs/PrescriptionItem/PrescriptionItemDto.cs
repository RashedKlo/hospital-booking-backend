using System;

namespace hospital_booking.Data.DTOs.PrescriptionItem
{
    public class PrescriptionItemDto
    {
        public int ItemId { get; set; }
        public int PrescriptionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
    }
}
