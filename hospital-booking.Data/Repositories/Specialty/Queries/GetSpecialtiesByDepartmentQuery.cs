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
    public static class GetSpecialtiesByDepartmentQuery
    {
        private const string GetSpecialtiesSql = @"
            SELECT * FROM specialties 
            WHERE department_id = @DepartmentId AND is_active = 1
            ORDER BY name";

        public static async Task<OperationResult<List<SpecialtyDto>>> ExecuteAsync(
            int departmentId,
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting specialties for department: {DepartmentId}", departmentId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSpecialtiesSql, connection);
                command.Parameters.AddWithValue("@DepartmentId", departmentId);

                var specialties = new List<SpecialtyDto>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var specialty = SpecialtyMapper.MapSpecialtyFromReader(reader);
                    specialties.Add(SpecialtyMapper.MapToDto(specialty));
                }
                logger.LogDebug("Retrieved {Count} specialties for department: {DepartmentId}", specialties.Count, departmentId);
                return OperationResult<List<SpecialtyDto>>.Success(specialties);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting specialties for department: {DepartmentId}", departmentId);
                return OperationResult<List<SpecialtyDto>>.Failure("Failed to retrieve specialties");
            }
        }
    }
}