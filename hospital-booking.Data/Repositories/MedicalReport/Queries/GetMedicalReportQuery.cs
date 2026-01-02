using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.MedicalReport.Helpers;

namespace hospital_booking.Data.Repositories.MedicalReport.Queries
{
    public class GetMedicalReportQuery
    {
        private const string GetSql = @"
SELECT 
    mr.report_id, mr.appointment_id, mr.diagnosis, mr.notes, mr.required_tests,
    a.appointment_id, a.patient_id, a.doctor_id, a.appointment_time, a.reason, a.status
FROM dbo.medical_reports mr
INNER JOIN dbo.appointments a ON mr.appointment_id = a.appointment_id
WHERE mr.report_id = @ReportId;
";

        public static async Task<OperationResult<MedicalReportDto>> ExecuteAsync(int reportId, ILogger logger)
        {
            if (reportId <= 0)
            {
                return OperationResult<MedicalReportDto>.Failure("Invalid report ID");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@ReportId", reportId);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<MedicalReportDto>.Failure("Medical report not found");
                }

                var dto = MedicalReportMapper.MapFromReader(reader);
                return OperationResult<MedicalReportDto>.Success(dto, "Medical report retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting medical report: {Error}", ex.Message);
                return OperationResult<MedicalReportDto>.Failure("Database operation failed");
            }
        }
    }
}
