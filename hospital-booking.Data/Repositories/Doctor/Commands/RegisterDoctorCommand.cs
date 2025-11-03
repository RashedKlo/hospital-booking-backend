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
    public static class RegisterDoctorCommand
    {
        private const string RegisterDoctorSql = @"
            INSERT INTO doctors (full_name, email, phone, password_hash, specialty_id, experience_years, bio, is_active, created_at)
            OUTPUT INSERTED.*
            VALUES (@FullName, @Email, @Phone, @PasswordHash, @SpecialtyId, @ExperienceYears, @Bio, 1, GETDATE())";

        public static async Task<OperationResult<DoctorAuthenticationData>> ExecuteAsync(
            CreateDoctorDto dto,
            ILogger logger,
            TokenHandler tokenHandler,
            string connectionString)
        {
            logger.LogInformation("Registering doctor: {Email}", dto.Email);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                using var command = new SqlCommand(RegisterDoctorSql, connection);
                command.Parameters.AddWithValue("@FullName", dto.FullName);
                command.Parameters.AddWithValue("@Email", dto.Email);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@SpecialtyId", dto.SpecialtyId);
                command.Parameters.AddWithValue("@ExperienceYears", (object?)dto.ExperienceYears ?? DBNull.Value);
                command.Parameters.AddWithValue("@Bio", (object?)dto.Bio ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("No result returned from doctor registration");
                    return OperationResult<DoctorAuthenticationData>.Failure("Registration failed");
                }

                var doctor = DoctorMapper.MapDoctorFromReader(reader);

                var accessToken = tokenHandler.GenerateAccessToken(doctor.Id, doctor.Email, doctor.FullName);
                var refreshToken = tokenHandler.GenerateRefreshToken();

                var authData = new DoctorAuthenticationData(doctor, accessToken, refreshToken);

                logger.LogInformation("Doctor registered successfully: {Id}", doctor.Id);
                return OperationResult<DoctorAuthenticationData>.Success(authData, "Doctor registered successfully");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                logger.LogWarning(ex, "Duplicate email during registration: {Email}", dto.Email);
                return OperationResult<DoctorAuthenticationData>.Failure("Email already exists");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                logger.LogWarning(ex, "Invalid specialty ID: {SpecialtyId}", dto.SpecialtyId);
                return OperationResult<DoctorAuthenticationData>.Failure("Specialty not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering doctor: {Email}", dto.Email);
                return OperationResult<DoctorAuthenticationData>.Failure("Registration failed");
            }
        }
    }
}