using hospital_booking.Data.DTOs.Appointment;

namespace hospital_booking.Data.DTOs.Prescription
{
    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public string Instructions { get; set; } = string.Empty;

        public AppointmentDto? Appointment { get; set; }
    }
}
