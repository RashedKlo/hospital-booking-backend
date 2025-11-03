using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Repositories.Admin.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Admin.Commands
{
    public static class UpdateAdminCommand
    {
        private const string UpdateAdminSql = @"
            UPDATE admins 
            SET full_name = @FullName,
                phone = @Phone,
                role = @Role,
                updated_at = GETDATE()
            OUTPUT INSERTED.*
            WHERE id = @AdminId AND is_active = 1";

        public static async Task<OperationResult<AdminDto>> ExecuteAsync(
            int adminId,
            UpdateAdminDto dto,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Updating admin: {Id}", adminId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateAdminSql, connection);
                command.Parameters.AddWithValue("@AdminId", adminId);
                command.Parameters.AddWithValue("@FullName", dto.FullName);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@Role", dto.Role);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Admin not found for update: {Id}", adminId);
                    return OperationResult<AdminDto>.Failure("Admin not found");
                }

                var admin = AdminMapper.MapAdminFromReader(reader);
                var adminDto = AdminMapper.MapToDto(admin);

                logger.LogInformation("Admin updated successfully: {Id}", adminId);
                return OperationResult<AdminDto>.Success(adminDto, "Admin updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating admin: {Id}", adminId);
                return OperationResult<AdminDto>.Failure("Update failed");
            }
        }
    }
}