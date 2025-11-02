using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Repositories.Department.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Department.Commands
{
    public static class UpdateDepartmentCommand
    {
        private const string UpdateDepartmentSql = @"
            UPDATE departments 
            SET name = @Name,
                description = @Description,
                updated_at = GETDATE()
            OUTPUT INSERTED.*
            WHERE id = @DepartmentId AND is_active = 1";

        public static async Task<OperationResult<DepartmentDto>> ExecuteAsync(
            int departmentId,
            UpdateDepartmentDto dto,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Updating department: {Id}", departmentId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateDepartmentSql, connection);
                command.Parameters.AddWithValue("@DepartmentId", departmentId);
                command.Parameters.AddWithValue("@Name", dto.Name);
                command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Department not found for update: {Id}", departmentId);
                    return OperationResult<DepartmentDto>.Failure("Department not found");
                }

                var department = DepartmentMapper.MapDepartmentFromReader(reader);
                var departmentDto = DepartmentMapper.MapToDto(department);

                logger.LogInformation("Department updated successfully: {Id}", departmentId);
                return OperationResult<DepartmentDto>.Success(departmentDto, "Department updated successfully");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                logger.LogWarning(ex, "Duplicate department name: {Name}", dto.Name);
                return OperationResult<DepartmentDto>.Failure("Department name already exists");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating department: {Id}", departmentId);
                return OperationResult<DepartmentDto>.Failure("Department update failed");
            }
        }
    }
}