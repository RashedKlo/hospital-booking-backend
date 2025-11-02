using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.Repositories.Specialty.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Specialty.Commands
{
    public static class CreateSpecialtyCommand
    {
        private const string CreateSpecialtySql = @"
            INSERT INTO specialties (department_id, name, description, is_active, created_at)
            OUTPUT INSERTED.*
            VALUES (@DepartmentId, @Name, @Description, 1, GETDATE())";

        public static async Task<OperationResult<SpecialtyDto>> ExecuteAsync(
            CreateSpecialtyDto dto,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Creating specialty: {Name}", dto.Name);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSpecialtySql, connection);
                command.Parameters.AddWithValue("@DepartmentId", dto.DepartmentId);
                command.Parameters.AddWithValue("@Name", dto.Name);
                command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("No result returned from specialty creation");
                    return OperationResult<SpecialtyDto>.Failure("Specialty creation failed");
                }

                var specialty = SpecialtyMapper.MapSpecialtyFromReader(reader);
                var specialtyDto = SpecialtyMapper.MapToDto(specialty);

                logger.LogInformation("Specialty created successfully: {Id}", specialty.Id);
                return OperationResult<SpecialtyDto>.Success(specialtyDto, "Specialty created successfully");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                logger.LogWarning(ex, "Duplicate specialty name: {Name}", dto.Name);
                return OperationResult<SpecialtyDto>.Failure("Specialty name already exists");
            }
            catch (SqlException ex) when (ex.Number == 547) // Foreign key violation
            {
                logger.LogWarning(ex, "Invalid department ID: {DepartmentId}", dto.DepartmentId);
                return OperationResult<SpecialtyDto>.Failure("Department not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating specialty: {Name}", dto.Name);
                return OperationResult<SpecialtyDto>.Failure("Specialty creation failed");
            }
        }
    }
}