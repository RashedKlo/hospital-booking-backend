using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Doctor.Helpers;

namespace hospital_booking.Data.Repositories.Doctor.Commands
{
    public static class UpdateDoctorCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.doctors
SET clinic_id = @ClinicId,
    full_name = @FullName,
    bio = @Bio,
    phone = @Phone,
    is_active = @IsActive,
    experience_years = @ExperienceYears
WHERE doctor_id = @DoctorId;

SELECT doctor_id, clinic_id, full_name, bio, phone, is_active, experience_years
FROM dbo.doctors
WHERE doctor_id = @DoctorId;
";

        public static async Task<OperationResult<DoctorDto>> ExecuteAsync(int doctorId, DoctorDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("UpdateDoctorCommand received null dto");
                return OperationResult<DoctorDto>.Failure("Doctor data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                command.Parameters.AddWithValue("@ClinicId", dto.ClinicId);
                command.Parameters.AddWithValue("@FullName", dto.FullName ?? string.Empty);
                command.Parameters.AddWithValue("@Bio", dto.Bio ?? string.Empty);
                command.Parameters.AddWithValue("@Phone", dto.Phone ?? string.Empty);
                command.Parameters.AddWithValue("@IsActive", dto.IsActive);
                command.Parameters.AddWithValue("@ExperienceYears", dto.ExperienceYears);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<DoctorDto>.Failure("Doctor not found");
                }

                var doctor = DoctorMapper.MapFromReader(reader);
                return OperationResult<DoctorDto>.Success(doctor, "Doctor updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating doctor: {Error}", ex.Message);
                return OperationResult<DoctorDto>.Failure("Database operation failed");
            }
        }
    }
}
