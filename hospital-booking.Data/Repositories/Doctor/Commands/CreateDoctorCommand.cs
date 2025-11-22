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
    public static class CreateDoctorCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.doctors (clinic_id, full_name, bio, phone, is_active, experience_years)
OUTPUT inserted.doctor_id, inserted.clinic_id, inserted.full_name, inserted.bio, inserted.phone, inserted.is_active, inserted.experience_years
VALUES (@ClinicId, @FullName, @Bio, @Phone, @IsActive, @ExperienceYears);
";

        public static async Task<OperationResult<DoctorDto>> ExecuteAsync(DoctorDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateDoctorCommand received null dto");
                return OperationResult<DoctorDto>.Failure("Doctor data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@ClinicId", dto.ClinicId);
                command.Parameters.AddWithValue("@FullName", dto.FullName ?? string.Empty);
                command.Parameters.AddWithValue("@Bio", dto.Bio ?? string.Empty);
                command.Parameters.AddWithValue("@Phone", dto.Phone ?? string.Empty);
                command.Parameters.AddWithValue("@IsActive", dto.IsActive);
                command.Parameters.AddWithValue("@ExperienceYears", dto.ExperienceYears);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<DoctorDto>.Failure("Doctor creation returned no result");
                }

                var doctor = DoctorMapper.MapFromReader(reader);
                return OperationResult<DoctorDto>.Success(doctor, "Doctor created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating doctor: {Error}", ex.Message);
                return OperationResult<DoctorDto>.Failure("Database operation failed");
            }
        }
    }
}
