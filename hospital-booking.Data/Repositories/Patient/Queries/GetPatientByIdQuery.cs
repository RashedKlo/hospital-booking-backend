using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Repositories.Patient.Helpers;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Patient.Queries
{
    public static class GetPatientByIdQuery
    {
        private const string GetPatientSql = @"
            SELECT * FROM patients 
            WHERE id = @PatientId AND is_active = 1";

        public static async Task<OperationResult<PatientProfileDto>> ExecuteAsync(int patientId, ILogger logger)
        {
            logger.LogDebug("Getting patient by ID: {PatientId}", patientId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetPatientSql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogDebug("Patient not found with ID: {PatientId}", patientId);
                    return OperationResult<PatientProfileDto>.Failure("Patient not found");
                }

                var patient = PatientMapper.MapPatientFromReader(reader);
                var profileDto = PatientMapper.MapToProfileDto(patient);

                logger.LogDebug("Patient found with ID: {PatientId}", patientId);
                return OperationResult<PatientProfileDto>.Success(profileDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting patient by ID: {PatientId}", patientId);
                return OperationResult<PatientProfileDto>.Failure("Failed to retrieve patient");
            }
        }
    }
}