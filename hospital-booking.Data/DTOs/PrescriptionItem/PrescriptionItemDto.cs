using hospital_booking.Data.DTOs.Prescription;

namespace hospital_booking.Data.DTOs.PrescriptionItem
{
    public class PrescriptionItemDto
    {
        public int PrescriptionItemId { get; set; }
        public int PrescriptionId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;

        public PrescriptionDto? Prescription { get; set; }
    }
}
