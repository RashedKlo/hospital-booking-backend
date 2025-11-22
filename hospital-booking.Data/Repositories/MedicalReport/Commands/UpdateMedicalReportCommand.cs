using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.MedicalReport.Helpers;

namespace hospital_booking.Data.Repositories.MedicalReport.Commands
{
    public static class UpdateMedicalReportCommand
    {
        private const string UpdateMedicalReportSql = @"
UPDATE dbo.medical_reports
SET appointment_id = @AppointmentId,
    diagnosis = @Diagnosis,
    notes = @Notes,
    required_tests = @RequiredTests
OUTPUT inserted.report_id, inserted.appointment_id, inserted.diagnosis, inserted.notes, inserted.required_tests
WHERE report_id = @ReportId;
";

        public static async Task<OperationResult<MedicalReportDto>> ExecuteAsync(
            int reportId,
            MedicalReportDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("UpdateMedicalReportCommand received null medical report data");
                return OperationResult<MedicalReportDto>.Failure("Medical report data is required");
            }

            if (reportId <= 0)
            {
                logger.LogError("UpdateMedicalReportCommand received invalid report ID: {ReportId}", reportId);
                return OperationResult<MedicalReportDto>.Failure("Invalid report ID");
            }

            logger.LogInformation("Executing medical report update for ReportId: {ReportId}", reportId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, reportId, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, reportId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during medical report update for ReportId: {ReportId}. Error: {Error}",
                    reportId, ex.Message);
                return OperationResult<MedicalReportDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during medical report update for ReportId: {ReportId}", reportId);
                return OperationResult<MedicalReportDto>.Failure("Medical report update failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int reportId, MedicalReportDto dto)
        {
            var command = new SqlCommand(UpdateMedicalReportSql, connection);
            command.Parameters.AddWithValue("@ReportId", reportId);
            command.Parameters.AddWithValue("@AppointmentId", dto.AppointmentId);
            command.Parameters.AddWithValue("@Diagnosis", dto.Diagnosis ?? string.Empty);
            command.Parameters.AddWithValue("@Notes", dto.Notes ?? string.Empty);
            command.Parameters.AddWithValue("@RequiredTests", dto.RequiredTests ?? string.Empty);
            return command;
        }

        private static async Task<OperationResult<MedicalReportDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int reportId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from medical report update for ReportId: {ReportId}", reportId);
                return OperationResult<MedicalReportDto>.Failure("Medical report not found or update failed");
            }

            var medicalReport = MedicalReportMapper.MapFromReader(reader);
            logger.LogInformation("Medical report updated successfully - ReportId: {ReportId}", medicalReport.ReportId);

            return OperationResult<MedicalReportDto>.Success(medicalReport, "Medical report updated successfully");
        }
    }
}
