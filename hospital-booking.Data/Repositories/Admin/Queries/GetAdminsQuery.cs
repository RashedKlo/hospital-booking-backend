using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Admin.Helpers;

namespace hospital_booking.Data.Repositories.Admin.Queries
{
    public class GetAdminsQuery
    {
        private const string GetSql = @"
SELECT
    admin_id,
    full_name,
    email,
    password,
    role,
    is_active
FROM dbo.admins
ORDER BY admin_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        public static async Task<OperationResult<List<AdminDto>>> ExecuteAsync(int page, int limit, ILogger logger)
        {
            if (page <= 0 || limit <= 0)
            {
                logger.LogError("GetAdminsQuery received invalid pagination");
                return OperationResult<List<AdminDto>>.Failure("Invalid pagination");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                var offset = (page - 1) * limit;
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Limit", limit);

                using var reader = await command.ExecuteReaderAsync();
                var list = new List<AdminDto>();
                while (await reader.ReadAsync())
                {
                    list.Add(AdminMapper.MapFromReader(reader));
                }

                return OperationResult<List<AdminDto>>.Success(list, "Admins retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting admins: {Error}", ex.Message);
                return OperationResult<List<AdminDto>>.Failure("Database operation failed");
            }
        }
    }
}
