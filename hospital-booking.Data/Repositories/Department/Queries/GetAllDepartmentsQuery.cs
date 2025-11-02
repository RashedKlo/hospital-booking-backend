using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Repositories.Department.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Department.Queries
{
    public static class GetAllDepartmentsQuery
    {
        private const string GetAllDepartmentsSql = @"
            SELECT * FROM departments 
            WHERE is_active = 1
            ORDER BY name";

        public static async Task<OperationResult<List<DepartmentDto>>> ExecuteAsync(
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting all departments");

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetAllDepartmentsSql, connection);
                var departments = new List<DepartmentDto>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var department = DepartmentMapper.MapDepartmentFromReader(reader);
                    departments.Add(DepartmentMapper.MapToDto(department));
                }

                logger.LogDebug("Retrieved {Count} departments", departments.Count);
                return OperationResult<List<DepartmentDto>>.Success(departments);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all departments");
                return OperationResult<List<DepartmentDto>>.Failure("Failed to retrieve departments");
            }
        }
    }
}