using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin; // For PaginationDto

namespace hospital_booking.Data.DTOs.ClinicService
{
    public class ClinicServicesDto
    {
        public List<ClinicServiceDto> Services { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
