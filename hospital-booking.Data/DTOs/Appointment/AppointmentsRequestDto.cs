using System;

namespace hospital_booking.Data.DTOs.Appointment
{
    public class AppointmentsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchQuery { get; set; }
        public string? Status { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
