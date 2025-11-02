using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Repositories.Department.Helpers;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Department.Queries
{
    public static class GetDepartmentByIdQuery
    {
        private const string GetDepartmentSql = @"
            SELECT * FROM departments 
            WHERE id = @DepartmentId AND is_active = 1";

        public static async Task<OperationResult<DepartmentDto>> ExecuteAsync(
            int departmentId,
            ILogger logger
            )
        {
            logger.LogDebug("Getting department by ID: {Id}", departmentId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetDepartmentSql, connection);
                command.Parameters.AddWithValue("@DepartmentId", departmentId);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogDebug("Department not found: {Id}", departmentId);
                    return OperationResult<DepartmentDto>.Failure("Department not found");
                }

                var department = DepartmentMapper.MapDepartmentFromReader(reader);
                var departmentDto = DepartmentMapper.MapToDto(department);

                return OperationResult<DepartmentDto>.Success(departmentDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting department by ID: {Id}", departmentId);
                return OperationResult<DepartmentDto>.Failure("Failed to retrieve department");
            }
        }
    }
}