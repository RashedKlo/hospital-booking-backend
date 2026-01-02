using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.ClinicReview
{
    public class ClinicReviewUpdateDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public byte? Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Review comment cannot exceed 1000 characters")]
        public string? ReviewComment { get; set; }
    }

}
