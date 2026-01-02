using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Admin.Helpers;

namespace hospital_booking.Data.Repositories.Admin.Queries
{
    public class GetAdminByNameQuery
    {
        private const string GetSql = @"
SELECT TOP (1)
    admin_id,
    full_name,
    email,
    role,
    phone,
    is_active,
    created_at,
    updated_at
FROM dbo.admins
WHERE full_name = @FullName;
";

        public static async Task<OperationResult<AdminDto>> ExecuteAsync(string fullName, ILogger logger)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                logger.LogError("GetAdminByNameQuery received invalid full name: {FullName}", fullName);
                return OperationResult<AdminDto>.Failure("Invalid full name");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@FullName", fullName);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<AdminDto>.Failure("Admin not found");
                }

                var dto = AdminMapper.MapFromReader(reader);
                return OperationResult<AdminDto>.Success(dto, "Admin retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting admin: {Error}", ex.Message);
                return OperationResult<AdminDto>.Failure("Database operation failed");
            }
        }
    }
}
