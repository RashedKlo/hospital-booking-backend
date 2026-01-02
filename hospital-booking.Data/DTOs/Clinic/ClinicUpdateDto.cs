using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Clinic
{
    public class ClinicUpdateDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(250, ErrorMessage = "Email cannot exceed 250 characters")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Invalid website URL")]
        [StringLength(250, ErrorMessage = "Website URL cannot exceed 250 characters")]
        public string? Website { get; set; }

        [Url(ErrorMessage = "Invalid image URL")]
        public string? ImageUrl { get; set; }

        [StringLength(200, ErrorMessage = "Opening hours cannot exceed 200 characters")]
        public string? OpeningHours { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}

