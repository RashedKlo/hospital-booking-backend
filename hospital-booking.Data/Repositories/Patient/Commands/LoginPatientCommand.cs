using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Repositories.Patient.Helpers;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Settings;


namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class LoginPatientCommand
    {
        private const string LoginPatientSql = @"
            SELECT * FROM patients 
            WHERE email = @Email AND is_active = 1";

        private const string UpdateLastLoginSql = @"
            UPDATE patients 
            SET last_login = GETDATE() 
            WHERE id = @PatientId";

        public static async Task<OperationResult<PatientAuthenticationData>> ExecuteAsync(
            PatientLoginDto dto,
            ILogger logger,
            TokenHandler tokenHandler)
        {
            if (dto == null)
            {
                logger.LogError("LoginPatientCommand received null login data");
                return OperationResult<PatientAuthenticationData>.Failure("Login data is required");
            }

            logger.LogInformation("Executing patient login for email: {Email}", dto.Email);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(LoginPatientSql, connection);
                command.Parameters.AddWithValue("@Email", dto.Email);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Login failed - patient not found: {Email}", dto.Email);
                    return OperationResult<PatientAuthenticationData>.Failure("Invalid email or password");
                }

                var patient = PatientMapper.MapPatientFromReader(reader);
                reader.Close();

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, patient.PasswordHash))
                {
                    logger.LogWarning("Login failed - invalid password for: {Email}", dto.Email);
                    return OperationResult<PatientAuthenticationData>.Failure("Invalid email or password");
                }

                // Update last login
                using var updateCommand = new SqlCommand(UpdateLastLoginSql, connection);
                updateCommand.Parameters.AddWithValue("@PatientId", patient.Id);
                await updateCommand.ExecuteNonQueryAsync();

                // Generate tokens
                var accessToken = tokenHandler.GenerateAccessToken(patient.Id, patient.Email, patient.FullName);
                var refreshToken = tokenHandler.GenerateRefreshToken();

                var authData = new PatientAuthenticationData(patient, accessToken, refreshToken);

                logger.LogInformation("Patient logged in successfully - PatientId: {PatientId}", patient.Id);

                return OperationResult<PatientAuthenticationData>.Success(authData, "Login successful");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during patient login for {Email}", dto.Email);
                return OperationResult<PatientAuthenticationData>.Failure("Login failed due to system error");
            }
        }
    }
}