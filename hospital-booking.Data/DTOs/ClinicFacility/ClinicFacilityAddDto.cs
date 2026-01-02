using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.ClinicFacility
{
    public class ClinicFacilityAddDto
    {
        [Required(ErrorMessage = "Clinic ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Clinic ID")]
        public int ClinicId { get; set; }

        [Required(ErrorMessage = "Facility title is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
        public string Title { get; set; } = string.Empty;
    }

}
