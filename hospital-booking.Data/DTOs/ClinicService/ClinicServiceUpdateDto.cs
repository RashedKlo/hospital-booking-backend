using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.ClinicService
{
    public class ClinicServiceUpdateDto
    {
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 250 characters")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1,000,000")]
        public decimal? Price { get; set; }
    }

}
