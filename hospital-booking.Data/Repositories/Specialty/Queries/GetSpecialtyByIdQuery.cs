using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.Repositories.Specialty.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Specialty.Queries
{
    public static class GetSpecialtyByIdQuery
    {
        private const string GetSpecialtySql = @"
            SELECT * FROM specialties 
            WHERE id = @SpecialtyId AND is_active = 1";

        public static async Task<OperationResult<SpecialtyDto>> ExecuteAsync(
            int specialtyId,
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting specialty by ID: {Id}", specialtyId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSpecialtySql, connection);
                command.Parameters.AddWithValue("@SpecialtyId", specialtyId);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogDebug("Specialty not found: {Id}", specialtyId);
                    return OperationResult<SpecialtyDto>.Failure("Specialty not found");
                }

                var specialty = SpecialtyMapper.MapSpecialtyFromReader(reader);
                var specialtyDto = SpecialtyMapper.MapToDto(specialty);

                return OperationResult<SpecialtyDto>.Success(specialtyDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting specialty by ID: {Id}", specialtyId);
                return OperationResult<SpecialtyDto>.Failure("Failed to retrieve specialty");
            }
        }
    }
}