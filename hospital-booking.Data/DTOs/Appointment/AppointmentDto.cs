using System;

namespace hospital_booking.Data.DTOs.Appointment
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
    }
}
