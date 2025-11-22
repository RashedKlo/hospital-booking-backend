using System;

namespace hospital_booking.Data.DTOs.MedicalReport
{
    public class MedicalReportDto
    {
        public int ReportId { get; set; }
        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string RequiredTests { get; set; } = string.Empty;
    }
}
