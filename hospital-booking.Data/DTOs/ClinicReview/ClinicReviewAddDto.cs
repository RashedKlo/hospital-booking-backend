using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.ClinicReview
{
    public class ClinicReviewAddDto
    {
        [Required(ErrorMessage = "Clinic ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Clinic ID")]
        public int ClinicId { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Patient ID")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public byte Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Review comment cannot exceed 1000 characters")]
        public string? ReviewComment { get; set; }
    }

}
