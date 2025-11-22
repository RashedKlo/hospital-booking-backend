using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.MedicalReport.Helpers;

namespace hospital_booking.Data.Repositories.MedicalReport.Queries
{
    public class GetMedicalReportsByAppointmentQuery
    {
        private const string GetMedicalReportsByAppointmentSql = @"
    SELECT
        report_id,
        appointment_id,
        diagnosis,
        notes,
        required_tests
    FROM dbo.medical_reports
    WHERE appointment_id = @AppointmentId
    ORDER BY report_id;
    ";

        public static async Task<OperationResult<List<MedicalReportDto>>> ExecuteAsync(
            int appointmentId,
            ILogger logger)
        {
            if (appointmentId <= 0)
            {
                logger.LogError("GetMedicalReportsByAppointmentQuery received invalid appointment ID: {AppointmentId}", appointmentId);
                return OperationResult<List<MedicalReportDto>>.Failure("Invalid appointment ID");
            }

            logger.LogInformation("Executing getting medical reports by AppointmentId: {AppointmentId}", appointmentId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, appointmentId);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, appointmentId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting medical reports by AppointmentId: {AppointmentId}. Error: {Error}",
                    appointmentId, ex.Message);
                return OperationResult<List<MedicalReportDto>>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting medical reports by AppointmentId: {AppointmentId}", appointmentId);
                return OperationResult<List<MedicalReportDto>>.Failure("Getting medical reports failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int appointmentId)
        {
            var command = new SqlCommand(GetMedicalReportsByAppointmentSql, connection);
            command.Parameters.AddWithValue("@AppointmentId", appointmentId);
            return command;
        }

        private static async Task<OperationResult<List<MedicalReportDto>>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int appointmentId)
        {
            var medicalReports = new List<MedicalReportDto>();

            while (await reader.ReadAsync())
            {
                medicalReports.Add(MedicalReportMapper.MapFromReader(reader));
            }

            logger.LogInformation("Retrieved {Count} medical reports for AppointmentId: {AppointmentId}",
                medicalReports.Count, appointmentId);

            return OperationResult<List<MedicalReportDto>>.Success(medicalReports, "Medical reports retrieved successfully");
        }
    }
}
