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
    public static class RegisterAdminCommand
    {
        private const string RegisterAdminSql = @"
            INSERT INTO admins (full_name, email, phone, password_hash, role, is_active, created_at)
            OUTPUT INSERTED.*
            VALUES (@FullName, @Email, @Phone, @PasswordHash, @Role, 1, GETDATE())";

        public static async Task<OperationResult<AdminAuthenticationData>> ExecuteAsync(
            CreateAdminDto dto,
            ILogger logger,
            TokenHandler tokenHandler,
            string connectionString)
        {
            logger.LogInformation("Registering admin: {Email}", dto.Email);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                using var command = new SqlCommand(RegisterAdminSql, connection);
                command.Parameters.AddWithValue("@FullName", dto.FullName);
                command.Parameters.AddWithValue("@Email", dto.Email);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@Role", dto.Role);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("No result returned from admin registration");
                    return OperationResult<AdminAuthenticationData>.Failure("Registration failed");
                }

                var admin = AdminMapper.MapAdminFromReader(reader);

                var accessToken = tokenHandler.GenerateAccessToken(admin.Id, admin.Email, admin.FullName);
                var refreshToken = tokenHandler.GenerateRefreshToken();

                var authData = new AdminAuthenticationData(admin, accessToken, refreshToken);

                logger.LogInformation("Admin registered successfully: {Id}", admin.Id);
                return OperationResult<AdminAuthenticationData>.Success(authData, "Admin registered successfully");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                logger.LogWarning(ex, "Duplicate email during registration: {Email}", dto.Email);
                return OperationResult<AdminAuthenticationData>.Failure("Email already exists");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering admin: {Email}", dto.Email);
                return OperationResult<AdminAuthenticationData>.Failure("Registration failed");
            }
        }
    }
}