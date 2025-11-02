using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.Repositories.Specialty.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Specialty.Commands
{
    public static class UpdateSpecialtyCommand
    {
        private const string UpdateSpecialtySql = @"
            UPDATE specialties 
            SET department_id = @DepartmentId,
                name = @Name,
                description = @Description,
                updated_at = GETDATE()
            OUTPUT INSERTED.*
            WHERE id = @SpecialtyId AND is_active = 1";

        public static async Task<OperationResult<SpecialtyDto>> ExecuteAsync(
            int specialtyId,
            UpdateSpecialtyDto dto,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Updating specialty: {Id}", specialtyId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSpecialtySql, connection);
                command.Parameters.AddWithValue("@SpecialtyId", specialtyId);
                command.Parameters.AddWithValue("@DepartmentId", dto.DepartmentId);
                command.Parameters.AddWithValue("@Name", dto.Name);
                command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Specialty not found for update: {Id}", specialtyId);
                    return OperationResult<SpecialtyDto>.Failure("Specialty not found");
                }

                var specialty = SpecialtyMapper.MapSpecialtyFromReader(reader);
                var specialtyDto = SpecialtyMapper.MapToDto(specialty);

                logger.LogInformation("Specialty updated successfully: {Id}", specialtyId);
                return OperationResult<SpecialtyDto>.Success(specialtyDto, "Specialty updated successfully");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                logger.LogWarning(ex, "Duplicate specialty name: {Name}", dto.Name);
                return OperationResult<SpecialtyDto>.Failure("Specialty name already exists");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                logger.LogWarning(ex, "Invalid department ID: {DepartmentId}", dto.DepartmentId);
                return OperationResult<SpecialtyDto>.Failure("Department not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating specialty: {Id}", specialtyId);
                return OperationResult<SpecialtyDto>.Failure("Specialty update failed");
            }
        }
    }
}