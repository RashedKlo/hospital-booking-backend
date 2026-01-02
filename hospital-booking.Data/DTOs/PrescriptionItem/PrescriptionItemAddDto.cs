using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.PrescriptionItem
{
    public class PrescriptionItemAddDto
    {
        [Required(ErrorMessage = "Prescription ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Prescription ID")]
        public int PrescriptionId { get; set; }

        [Required(ErrorMessage = "Medication name is required")]
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

