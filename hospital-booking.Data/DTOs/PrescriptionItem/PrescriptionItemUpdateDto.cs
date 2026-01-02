using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.PrescriptionItem
{
    public class PrescriptionItemUpdateDto
    {
        [StringLength(250, ErrorMessage = "Medication name cannot exceed 250 characters")]
        public string? MedicationName { get; set; }

        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters")]
        public string? Dosage { get; set; }

        [StringLength(100, ErrorMessage = "Duration cannot exceed 100 characters")]
        public string? Duration { get; set; }

        [StringLength(100, ErrorMessage = "Frequency cannot exceed 100 characters")]
        public string? Frequency { get; set; }
    }
}

