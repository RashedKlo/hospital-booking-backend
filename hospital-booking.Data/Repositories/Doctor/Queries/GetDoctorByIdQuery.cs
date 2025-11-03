using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Repositories.Doctor.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Doctor.Queries
{
    public static class GetDoctorByIdQuery
    {
        private const string GetDoctorSql = @"
            SELECT * FROM doctors 
            WHERE id = @DoctorId AND is_active = 1";

        public static async Task<OperationResult<DoctorDto>> ExecuteAsync(
            int doctorId,
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting doctor by ID: {Id}", doctorId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetDoctorSql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogDebug("Doctor not found: {Id}", doctorId);
                    return OperationResult<DoctorDto>.Failure("Doctor not found");
                }

                var doctor = DoctorMapper.MapDoctorFromReader(reader);
                var doctorDto = DoctorMapper.MapToDto(doctor);

                return OperationResult<DoctorDto>.Success(doctorDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting doctor by ID: {Id}", doctorId);
                return OperationResult<DoctorDto>.Failure("Failed to retrieve doctor");
            }
        }
    }
}