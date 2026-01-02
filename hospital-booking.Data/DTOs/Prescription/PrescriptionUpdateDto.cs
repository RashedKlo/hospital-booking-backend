using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Prescription
{
    public class PrescriptionUpdateDto
    {
        [MinLength(10, ErrorMessage = "Instructions must be at least 10 characters long")]
        public string? Instructions { get; set; }
    }
}
