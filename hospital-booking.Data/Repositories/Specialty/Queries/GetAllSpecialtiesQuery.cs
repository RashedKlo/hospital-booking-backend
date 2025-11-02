using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.Repositories.Specialty.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Specialty.Queries
{
    public static class GetAllSpecialtiesQuery
    {
        private const string GetAllSpecialtiesSql = @"
            SELECT * FROM specialties 
            WHERE is_active = 1
            ORDER BY name";

        public static async Task<OperationResult<List<SpecialtyDto>>> ExecuteAsync(
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting all specialties");

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetAllSpecialtiesSql, connection);
                var specialties = new List<SpecialtyDto>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var specialty = SpecialtyMapper.MapSpecialtyFromReader(reader);
                    specialties.Add(SpecialtyMapper.MapToDto(specialty));
                }

                logger.LogDebug("Retrieved {Count} specialties", specialties.Count);
                return OperationResult<List<SpecialtyDto>>.Success(specialties);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all specialties");
                return OperationResult<List<SpecialtyDto>>.Failure("Failed to retrieve specialties");
            }
        }
    }
}