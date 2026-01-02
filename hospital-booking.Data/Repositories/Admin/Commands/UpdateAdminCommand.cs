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
    role = @Role,
    phone = @Phone,
    is_active = @IsActive,
    updated_at = GETDATE()
WHERE admin_id = @AdminId;

SELECT admin_id, user_id, full_name, role, phone, is_active, created_at, updated_at
FROM dbo.admins
WHERE admin_id = @AdminId;
";

        public static async Task<OperationResult<AdminDto>> ExecuteAsync(int adminId, AdminUpdateDto dto, ILogger logger)
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
                command.Parameters.AddWithValue("@Role", dto.Role ?? string.Empty);
                command.Parameters.AddWithValue("@Phone", (object?)dto.Phone ?? DBNull.Value);
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
