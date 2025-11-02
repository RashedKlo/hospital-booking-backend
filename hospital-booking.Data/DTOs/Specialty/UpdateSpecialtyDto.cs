using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Specialty
{
    public class UpdateSpecialtyDto
    {
        [Required(ErrorMessage = "Department ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Department ID must be a positive number")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Specialty name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Specialty name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}