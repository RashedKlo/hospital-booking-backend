using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Repositories.Patient.Helpers;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Patient.Queries
{
    public static class GetAllPatientsQuery
    {
        private const string GetAllPatientsSql = @"
            SELECT * FROM patients 
            WHERE is_active = 1
            ORDER BY created_at DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        public static async Task<OperationResult<List<PatientProfileDto>>> ExecuteAsync(
            int pageNumber, 
            int pageSize, 
            ILogger logger)
        {
            logger.LogDebug("Getting all patients - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetAllPatientsSql, connection);
                command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                var patients = new List<PatientProfileDto>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var patient = PatientMapper.MapPatientFromReader(reader);
                    patients.Add(PatientMapper.MapToProfileDto(patient));
                }

                logger.LogDebug("Retrieved {Count} patients", patients.Count);
                return OperationResult<List<PatientProfileDto>>.Success(patients);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all patients");
                return OperationResult<List<PatientProfileDto>>.Failure("Failed to retrieve patients");
            }
        }
    }
}