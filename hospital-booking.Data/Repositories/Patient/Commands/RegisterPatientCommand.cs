using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Repositories.Patient.Helpers;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;

namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class RegisterPatientCommand
    {
        private const string RegisterPatientSql = @"
            INSERT INTO patients (full_name, email, phone, date_of_birth, password_hash, is_google_login, is_email_verified, is_active, created_at)
            OUTPUT INSERTED.*
            VALUES (@FullName, @Email, @Phone, @DateOfBirth, @PasswordHash, @IsGoogleLogin, 0, 1, GETDATE())";

        public static async Task<OperationResult<PatientAuthenticationData>> ExecuteAsync(
            PatientRegistrationDto dto,
            ILogger logger,
            TokenHandler tokenHandler)
        {
            if (dto == null)
            {
                logger.LogError("RegisterPatientCommand received null registration data");
                return OperationResult<PatientAuthenticationData>.Failure("Registration data is required");
            }

            logger.LogInformation("Executing patient registration for email: {Email}", dto.Email);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                // Hash password if not Google login
                string passwordHash = string.Empty;
                if (!dto.IsGoogleLogin && !string.IsNullOrEmpty(dto.Password))
                {
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                using var command = CreateCommand(connection, dto, passwordHash);
                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("No result returned from registration for {Email}", dto.Email);
                    return OperationResult<PatientAuthenticationData>.Failure("Registration failed");
                }

                var patient = PatientMapper.MapPatientFromReader(reader);
                
                // Generate tokens
                var accessToken = tokenHandler.GenerateAccessToken(patient.Id, patient.Email, patient.FullName);
                var refreshToken =  tokenHandler.GenerateRefreshToken();

                var authData = new PatientAuthenticationData(patient, accessToken, refreshToken);

                logger.LogInformation("Patient registered successfully - PatientId: {PatientId}", patient.Id);

                return OperationResult<PatientAuthenticationData>.Success(authData, "Patient registered successfully");
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                logger.LogWarning(ex, "Duplicate email during registration: {Email}", dto.Email);
                return OperationResult<PatientAuthenticationData>.Failure("Email already exists");
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during patient registration for {Email}", dto.Email);
                return OperationResult<PatientAuthenticationData>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during patient registration for {Email}", dto.Email);
                return OperationResult<PatientAuthenticationData>.Failure("Registration failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, PatientRegistrationDto dto, string passwordHash)
        {
            var command = new SqlCommand(RegisterPatientSql, connection);
            command.Parameters.AddWithValue("@FullName", dto.FullName);
            command.Parameters.AddWithValue("@Email", dto.Email);
            command.Parameters.AddWithValue("@Phone", dto.Phone);
            command.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@IsGoogleLogin", dto.IsGoogleLogin);
            return command;
        }
    }
}