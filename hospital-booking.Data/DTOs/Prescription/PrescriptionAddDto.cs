using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Prescription
{
    public class PrescriptionAddDto
    {
         [Required(ErrorMessage = "Appointment ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Appointment ID")]

        public int AppointmentId { get; set; }

        [MinLength(10, ErrorMessage = "Instructions must be at least 10 characters long")]
        public string? Instructions { get; set; }
    }
}
