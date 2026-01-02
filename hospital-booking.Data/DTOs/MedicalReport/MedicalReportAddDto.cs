using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.MedicalReport
{
    public class MedicalReportAddDto
    {
        [Required(ErrorMessage = "Appointment ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Appointment ID")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Diagnosis is required")]
        [MinLength(5, ErrorMessage = "Diagnosis must be at least 5 characters long")]
        public string? Diagnosis { get; set; }

        public string? Notes { get; set; }

        [StringLength(1000, ErrorMessage = "Required tests must not exceed 1000 characters.")]
        public string? RequiredTests { get; set; }
    }
}
