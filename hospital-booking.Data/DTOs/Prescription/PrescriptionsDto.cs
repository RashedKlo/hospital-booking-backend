using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin;

namespace hospital_booking.Data.DTOs.Prescription
{
    public class PrescriptionsDto
    {
        public List<PrescriptionDto> Prescriptions { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
