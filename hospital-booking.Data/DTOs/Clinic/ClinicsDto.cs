using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin; // For PaginationDto

namespace hospital_booking.Data.DTOs.Clinic
{
    public class ClinicsDto
    {
        public List<ClinicDto> Clinics { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
