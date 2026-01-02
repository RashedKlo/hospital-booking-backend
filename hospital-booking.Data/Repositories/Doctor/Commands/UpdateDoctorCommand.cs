using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Doctor.Queries;

namespace hospital_booking.Data.Repositories.Doctor.Commands
{
    public static class UpdateDoctorCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.doctors
SET full_name = ISNULL(@FullName, full_name),
    bio = ISNULL(@Bio, bio),
    phone = ISNULL(@Phone, phone),
    is_active = ISNULL(@IsActive, is_active),
    experience_years = ISNULL(@ExperienceYears, experience_years)
WHERE doctor_id = @DoctorId;
";

        public static async Task<OperationResult<DoctorDto>> ExecuteAsync(int doctorId, DoctorUpdateDto dto, ILogger logger)
        {
            if (doctorId <= 0) return OperationResult<DoctorDto>.Failure("Invalid doctor id");
            if (dto == null) return OperationResult<DoctorDto>.Failure("Data is required");

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                command.Parameters.AddWithValue("@FullName", (object?)dto.FullName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Bio", (object?)dto.Bio ?? DBNull.Value);
                command.Parameters.AddWithValue("@Phone", (object?)dto.Phone ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", (object?)dto.IsActive ?? DBNull.Value);
                command.Parameters.AddWithValue("@ExperienceYears", (object?)dto.ExperienceYears ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<DoctorDto>.Failure("Doctor not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating doctor: {Error}", ex.Message);
                return OperationResult<DoctorDto>.Failure("Database operation failed");
            }

            return await GetDoctorQuery.ExecuteAsync(doctorId, logger);
        }
    }
}
