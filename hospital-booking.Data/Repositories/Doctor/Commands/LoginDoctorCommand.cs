using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Repositories.Doctor.Helpers;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;

namespace hospital_booking.Data.Repositories.Doctor.Commands
{
    public static class LoginDoctorCommand
    {
        private const string LoginDoctorSql = @"
            SELECT * FROM doctors 
            WHERE email = @Email AND is_active = 1";

        public static async Task<OperationResult<DoctorAuthenticationData>> ExecuteAsync(
            DoctorLoginDto dto,
            ILogger logger,
            TokenHandler tokenHandler,
            string connectionString)
        {
            logger.LogInformation("Doctor login attempt: {Email}", dto.Email);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(LoginDoctorSql, connection);
                command.Parameters.AddWithValue("@Email", dto.Email);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Login failed - doctor not found: {Email}", dto.Email);
                    return OperationResult<DoctorAuthenticationData>.Failure("Invalid email or password");
                }

                var doctor = DoctorMapper.MapDoctorFromReader(reader);

                if (!BCrypt.Net.BCrypt.Verify(dto.Password, doctor.PasswordHash))
                {
                    logger.LogWarning("Login failed - invalid password for: {Email}", dto.Email);
                    return OperationResult<DoctorAuthenticationData>.Failure("Invalid email or password");
                }

                var accessToken = tokenHandler.GenerateAccessToken(doctor.Id, doctor.Email, doctor.FullName);
                var refreshToken = tokenHandler.GenerateRefreshToken();

                var authData = new DoctorAuthenticationData(doctor, accessToken, refreshToken);

                logger.LogInformation("Doctor logged in successfully: {Id}", doctor.Id);
                return OperationResult<DoctorAuthenticationData>.Success(authData, "Login successful");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during doctor login: {Email}", dto.Email);
                return OperationResult<DoctorAuthenticationData>.Failure("Login failed");
            }
        }
    }
}