using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Repositories.Patient.Helpers;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class UpdatePatientCommand
    {
        private const string UpdatePatientSql = @"
            UPDATE patients 
            SET full_name = @FullName,
                phone = @Phone,
                date_of_birth = @DateOfBirth,
                updated_at = GETDATE()
            OUTPUT INSERTED.*
            WHERE id = @PatientId AND is_active = 1";

        public static async Task<OperationResult<PatientProfileDto>> ExecuteAsync(
            int patientId,
            PatientUpdateDto dto,
            ILogger logger)
        {
            logger.LogInformation("Updating patient: {PatientId}", patientId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, patientId, dto);
                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Patient not found for update: {PatientId}", patientId);
                    return OperationResult<PatientProfileDto>.Failure("Patient not found");
                }

                var patient = PatientMapper.MapPatientFromReader(reader);
                var profileDto = PatientMapper.MapToProfileDto(patient);

                logger.LogInformation("Patient updated successfully: {PatientId}", patientId);

                return OperationResult<PatientProfileDto>.Success(profileDto, "Patient updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating patient: {PatientId}", patientId);
                return OperationResult<PatientProfileDto>.Failure("Update failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, int patientId, PatientUpdateDto dto)
        {
            var command = new SqlCommand(UpdatePatientSql, connection);
            command.Parameters.AddWithValue("@PatientId", patientId);
            command.Parameters.AddWithValue("@FullName", dto.FullName);
            command.Parameters.AddWithValue("@Phone", dto.Phone);
            command.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
            return command;
        }
    }
}