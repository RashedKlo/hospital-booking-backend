using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Repositories.Doctor.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Doctor.Commands
{
    public static class UpdateDoctorCommand
    {
        private const string UpdateDoctorSql = @"
            UPDATE doctors 
            SET full_name = @FullName,
                phone = @Phone,
                specialty_id = @SpecialtyId,
                experience_years = @ExperienceYears,
                bio = @Bio,
                updated_at = GETDATE()
            OUTPUT INSERTED.*
            WHERE id = @DoctorId AND is_active = 1";

        public static async Task<OperationResult<DoctorDto>> ExecuteAsync(
            int doctorId,
            UpdateDoctorDto dto,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Updating doctor: {Id}", doctorId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateDoctorSql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                command.Parameters.AddWithValue("@FullName", dto.FullName);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@SpecialtyId", dto.SpecialtyId);
                command.Parameters.AddWithValue("@ExperienceYears", (object?)dto.ExperienceYears ?? DBNull.Value);
                command.Parameters.AddWithValue("@Bio", (object?)dto.Bio ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    logger.LogWarning("Doctor not found for update: {Id}", doctorId);
                    return OperationResult<DoctorDto>.Failure("Doctor not found");
                }

                var doctor = DoctorMapper.MapDoctorFromReader(reader);
                var doctorDto = DoctorMapper.MapToDto(doctor);

                logger.LogInformation("Doctor updated successfully: {Id}", doctorId);
                return OperationResult<DoctorDto>.Success(doctorDto, "Doctor updated successfully");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                logger.LogWarning(ex, "Invalid specialty ID: {SpecialtyId}", dto.SpecialtyId);
                return OperationResult<DoctorDto>.Failure("Specialty not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating doctor: {Id}", doctorId);
                return OperationResult<DoctorDto>.Failure("Update failed");
            }
        }
    }
}