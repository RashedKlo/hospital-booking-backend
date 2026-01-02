using System;
using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.Appointment
{
    public class AppointmentAddDto
    {
        [Required(ErrorMessage = "Patient ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Patient ID")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Doctor ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Doctor ID")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment time is required")]
        public DateTime AppointmentTime { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "pending";
    }
}

