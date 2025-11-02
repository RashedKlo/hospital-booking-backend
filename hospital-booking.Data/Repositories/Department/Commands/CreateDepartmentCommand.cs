using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Repositories.Department.Helpers;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Department.Commands
{
    public static class CreateDepartmentCommand
    {
        private const string CreateDepartmentSql = @"
            INSERT INTO departments (name, description, is_active, created_at)
            OUTPUT INSERTED.*
            VALUES (@Name, @Description, 1, GETDATE())";

        public static async Task<OperationResult<DepartmentDto>> ExecuteAsync(
            CreateDepartmentDto dto,
            ILogger logger)
        {
            logger.LogInformation("Creating department: {Name}", dto.Name);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateDepartmentSql, connection);
                command.Parameters.AddWithValue("@Name", dto.Name);
                command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("No result returned from department creation");
                    return OperationResult<DepartmentDto>.Failure("Department creation failed");
                }

                var department = DepartmentMapper.MapDepartmentFromReader(reader);
                var departmentDto = DepartmentMapper.MapToDto(department);

                logger.LogInformation("Department created successfully: {Id}", department.Id);
                return OperationResult<DepartmentDto>.Success(departmentDto, "Department created successfully");
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                logger.LogWarning(ex, "Duplicate department name: {Name}", dto.Name);
                return OperationResult<DepartmentDto>.Failure("Department name already exists");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating department: {Name}", dto.Name);
                return OperationResult<DepartmentDto>.Failure("Department creation failed");
            }
        }
    }
}