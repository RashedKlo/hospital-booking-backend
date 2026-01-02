using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Doctor.Commands
{
    public static class CreateDoctorCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.doctors (clinic_id, full_name, bio, phone, is_active, experience_years)
VALUES (@ClinicId, @FullName, @Bio, @Phone, 1, @ExperienceYears);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(DoctorAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateDoctorCommand received null dto");
                return OperationResult<bool>.Failure("Doctor data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@ClinicId", dto.ClinicId);
                command.Parameters.AddWithValue("@FullName", dto.FullName);
                command.Parameters.AddWithValue("@Bio", (object?)dto.Bio ?? DBNull.Value);
                command.Parameters.AddWithValue("@Phone", (object?)dto.Phone ?? DBNull.Value);
                command.Parameters.AddWithValue("@ExperienceYears", dto.ExperienceYears);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Doctor created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create doctor");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating doctor: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
