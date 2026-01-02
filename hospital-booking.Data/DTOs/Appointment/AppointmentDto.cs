using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.DTOs.Doctor;
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
        
        public PatientDto? Patient { get; set; }
        public DoctorDto? Doctor { get; set; }
    }
}
