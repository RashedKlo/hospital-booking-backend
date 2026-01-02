using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin; // For PaginationDto

namespace hospital_booking.Data.DTOs.Doctor
{
    public class DoctorsDto
    {
        public List<DoctorDto> Doctors { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
