using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.DTOs.Appointment;

namespace hospital_booking.Data.Repositories.MedicalReport.Helpers
{
    public static class MedicalReportMapper
    {
        public static MedicalReportDto MapFromReader(SqlDataReader reader)
        {
            var report = new MedicalReportDto
            {
                ReportId = reader.GetInt32(0),
                AppointmentId = reader.GetInt32(1),
                Diagnosis = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Notes = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                RequiredTests = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            };

            // Check if Appointment columns are present (start index 5)
            if (reader.FieldCount > 5)
            {
                report.Appointment = new AppointmentDto
                {
                    AppointmentId = reader.GetInt32(5),
                    PatientId = reader.GetInt32(6),
                    DoctorId = reader.GetInt32(7),
                    AppointmentTime = reader.GetDateTime(8),
                    Reason = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    Status = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                };
            }

            return report;
        }
    }
}
