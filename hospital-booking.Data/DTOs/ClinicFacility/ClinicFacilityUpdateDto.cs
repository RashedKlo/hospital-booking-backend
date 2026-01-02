using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.ClinicFacility
{
    public class ClinicFacilityUpdateDto
    {
        [Required(ErrorMessage = "Facility title is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
        public string Title { get; set; } = string.Empty;
    }

}
