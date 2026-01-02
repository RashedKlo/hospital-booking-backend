using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.MedicalReport
{
    public class MedicalReportUpdateDto
    {
        [StringLength(1000, ErrorMessage = "Diagnosis must not exceed 1000 characters.")]
        public string? Diagnosis { get; set; }

        [StringLength(2000, ErrorMessage = "Notes must not exceed 2000 characters.")]
        public string? Notes { get; set; }

        [StringLength(1000, ErrorMessage = "Required tests must not exceed 1000 characters.")]
        public string? RequiredTests { get; set; }
    }
}
