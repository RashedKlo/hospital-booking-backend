using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Admin.Helpers;

namespace hospital_booking.Data.Repositories.Admin.Commands
{
    public static class UpdateAdminCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.admins
SET full_name = @FullName,
    email = @Email,
    password = @Password,
    role = @Role,
    is_active = @IsActive
WHERE admin_id = @AdminId;

SELECT admin_id, full_name, email, password, role, is_active
FROM dbo.admins
WHERE admin_id = @AdminId;
";

        public static async Task<OperationResult<AdminDto>> ExecuteAsync(int adminId, AdminDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("UpdateAdminCommand received null dto");
                return OperationResult<AdminDto>.Failure("Admin data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@AdminId", adminId);
                command.Parameters.AddWithValue("@FullName", dto.FullName ?? string.Empty);
                command.Parameters.AddWithValue("@Email", dto.Email ?? string.Empty);
                command.Parameters.AddWithValue("@Password", (object?)dto.Password ?? DBNull.Value);
                command.Parameters.AddWithValue("@Role", dto.Role ?? string.Empty);
                command.Parameters.AddWithValue("@IsActive", dto.IsActive);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<AdminDto>.Failure("Admin not found");
                }

                var admin = AdminMapper.MapFromReader(reader);
                return OperationResult<AdminDto>.Success(admin, "Admin updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating admin: {Error}", ex.Message);
                return OperationResult<AdminDto>.Failure("Database operation failed");
            }
        }
    }
}
