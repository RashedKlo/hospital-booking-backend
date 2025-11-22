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
    public static class CreateMedicalReportCommand
    {
        private const string CreateMedicalReportSql = @"
INSERT INTO dbo.medical_reports (appointment_id, diagnosis, notes, required_tests)
OUTPUT inserted.report_id, inserted.appointment_id, inserted.diagnosis, inserted.notes, inserted.required_tests
VALUES (@AppointmentId, @Diagnosis, @Notes, @RequiredTests);
";

        public static async Task<OperationResult<MedicalReportDto>> ExecuteAsync(
            MedicalReportDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateMedicalReportCommand received null medical report data");
                return OperationResult<MedicalReportDto>.Failure("Medical report data is required");
            }

            logger.LogInformation("Executing medical report creation for AppointmentId: {AppointmentId}", dto.AppointmentId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, dto.AppointmentId);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during medical report creation for AppointmentId: {AppointmentId}. Error: {Error}",
                    dto.AppointmentId, ex.Message);
                return OperationResult<MedicalReportDto>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during medical report creation for AppointmentId: {AppointmentId}", dto.AppointmentId);
                return OperationResult<MedicalReportDto>.Failure("Medical report creation failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, MedicalReportDto dto)
        {
            var command = new SqlCommand(CreateMedicalReportSql, connection);
            command.Parameters.AddWithValue("@AppointmentId", dto.AppointmentId);
            command.Parameters.AddWithValue("@Diagnosis", dto.Diagnosis ?? string.Empty);
            command.Parameters.AddWithValue("@Notes", dto.Notes ?? string.Empty);
            command.Parameters.AddWithValue("@RequiredTests", dto.RequiredTests ?? string.Empty);
            return command;
        }

        private static async Task<OperationResult<MedicalReportDto>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            int appointmentId)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from medical report creation for AppointmentId: {AppointmentId}", appointmentId);
                return OperationResult<MedicalReportDto>.Failure("Medical report creation returned no result");
            }

            var medicalReport = MedicalReportMapper.MapFromReader(reader);
            logger.LogInformation("Medical report created successfully - ReportId: {ReportId}", medicalReport.ReportId);

            return OperationResult<MedicalReportDto>.Success(medicalReport, "Medical report created successfully");
        }
    }
}
