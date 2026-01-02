using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin;

namespace hospital_booking.Data.DTOs.Patient
{
    public class PatientsDto
    {
        public List<PatientDto> Patients { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
