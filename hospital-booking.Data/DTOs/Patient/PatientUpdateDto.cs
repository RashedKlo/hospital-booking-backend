using System;
using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Patient
{
    public class PatientUpdateDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 200 characters.")]
        public string? FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(10, ErrorMessage = "Gender must not exceed 10 characters.")]
        public string? Gender { get; set; }

        [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}
