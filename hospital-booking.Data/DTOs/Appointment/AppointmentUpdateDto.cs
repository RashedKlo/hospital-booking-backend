using System;
using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Appointment
{
    public class AppointmentUpdateDto
    {
        public DateTime? AppointmentTime { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string? Status { get; set; }
    }
}

