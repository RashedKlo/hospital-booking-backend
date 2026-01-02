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
    public static class CreateAdminCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.admins (full_name, email, role, phone, is_active)
OUTPUT inserted.admin_id, inserted.full_name, inserted.email, inserted.role, inserted.phone, inserted.is_active, inserted.created_at, inserted.updated_at
VALUES (@FullName, @Email, @Role, @Phone, @IsActive);
";

        public static async Task<OperationResult<AdminDto>> ExecuteAsync(AdminAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateAdminCommand received null dto");
                return OperationResult<AdminDto>.Failure("Admin data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@FullName", dto.FullName);
                command.Parameters.AddWithValue("@Email", dto.Email);
                command.Parameters.AddWithValue("@Role", dto.Role);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@IsActive", true);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<AdminDto>.Failure("Admin creation returned no result");
                }

                var admin = AdminMapper.MapFromReader(reader);
                return OperationResult<AdminDto>.Success(admin, "Admin created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating admin: {Error}", ex.Message);
                return OperationResult<AdminDto>.Failure("Database operation failed");
            }
        }
    }
}
