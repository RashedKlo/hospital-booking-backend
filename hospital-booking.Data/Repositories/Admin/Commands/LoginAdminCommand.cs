using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Repositories.Admin.Helpers;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;

namespace hospital_booking.Data.Repositories.Admin.Commands
{
    public static class LoginAdminCommand
    {
        private const string LoginAdminSql = @"
            SELECT * FROM admins 
            WHERE email = @Email AND is_active = 1";

        public static async Task<OperationResult<AdminAuthenticationData>> ExecuteAsync(
            AdminLoginDto dto,
            ILogger logger,
            TokenHandler tokenHandler,
            string connectionString)
        {
            logger.LogInformation("Admin login attempt: {Email}", dto.Email);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(LoginAdminSql, connection);
                command.Parameters.AddWithValue("@Email", dto.Email);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Login failed - admin not found: {Email}", dto.Email);
                    return OperationResult<AdminAuthenticationData>.Failure("Invalid email or password");
                }

                var admin = AdminMapper.MapAdminFromReader(reader);

                if (!BCrypt.Net.BCrypt.Verify(dto.Password, admin.PasswordHash))
                {
                    logger.LogWarning("Login failed - invalid password for: {Email}", dto.Email);
                    return OperationResult<AdminAuthenticationData>.Failure("Invalid email or password");
                }

                var accessToken = tokenHandler.GenerateAccessToken(admin.Id, admin.Email, admin.FullName);
                var refreshToken = tokenHandler.GenerateRefreshToken();

                var authData = new AdminAuthenticationData(admin, accessToken, refreshToken);

                logger.LogInformation("Admin logged in successfully: {Id}", admin.Id);
                return OperationResult<AdminAuthenticationData>.Success(authData, "Login successful");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during admin login: {Email}", dto.Email);
                return OperationResult<AdminAuthenticationData>.Failure("Login failed");
            }
        }
    }
}