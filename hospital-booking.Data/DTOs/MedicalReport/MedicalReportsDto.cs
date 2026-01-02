using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin;

namespace hospital_booking.Data.DTOs.MedicalReport
{
    public class MedicalReportsDto
    {
        public List<MedicalReportDto> Reports { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
