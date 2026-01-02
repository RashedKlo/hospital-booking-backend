using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin; // For PaginationDto

namespace hospital_booking.Data.DTOs.Appointment
{
    public class AppointmentsDto
    {
        public List<AppointmentDto> Appointments { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
