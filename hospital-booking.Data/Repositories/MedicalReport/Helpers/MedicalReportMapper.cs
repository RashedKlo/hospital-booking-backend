using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.MedicalReport;

namespace hospital_booking.Data.Repositories.MedicalReport.Helpers
{
    public static class MedicalReportMapper
    {
        public static MedicalReportDto MapFromReader(SqlDataReader reader)
        {
            return new MedicalReportDto
            {
                ReportId = reader.GetInt32(0),
                AppointmentId = reader.GetInt32(1),
                Diagnosis = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Notes = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                RequiredTests = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            };
        }
    }
}
