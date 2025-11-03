using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Repositories.Admin.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Admin.Queries
{
    public static class GetAdminByIdQuery
    {
        private const string GetAdminSql = @"
            SELECT * FROM admins 
            WHERE id = @AdminId AND is_active = 1";

        public static async Task<OperationResult<AdminDto>> ExecuteAsync(
            int adminId,
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting admin by ID: {Id}", adminId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetAdminSql, connection);
                command.Parameters.AddWithValue("@AdminId", adminId);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogDebug("Admin not found: {Id}", adminId);
                    return OperationResult<AdminDto>.Failure("Admin not found");
                }

                var admin = AdminMapper.MapAdminFromReader(reader);
                var adminDto = AdminMapper.MapToDto(admin);

                return OperationResult<AdminDto>.Success(adminDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting admin by ID: {Id}", adminId);
                return OperationResult<AdminDto>.Failure("Failed to retrieve admin");
            }
        }
    }
}