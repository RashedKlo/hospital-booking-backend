using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Doctor.Helpers;

namespace hospital_booking.Data.Repositories.Doctor.Queries
{
    public class GetDoctorQuery
    {
        private const string GetSql = @"
SELECT TOP (1)
    doctor_id,
    clinic_id,
    full_name,
    bio,
    phone,
    is_active,
    experience_years
FROM dbo.doctors
WHERE doctor_id = @DoctorId;
";

        public static async Task<OperationResult<DoctorDto>> ExecuteAsync(int doctorId, ILogger logger)
        {
            if (doctorId <= 0)
            {
                logger.LogError("GetDoctorQuery received invalid id: {DoctorId}", doctorId);
                return OperationResult<DoctorDto>.Failure("Invalid doctor id");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<DoctorDto>.Failure("Doctor not found");
                }

                var dto = DoctorMapper.MapFromReader(reader);
                return OperationResult<DoctorDto>.Success(dto, "Doctor retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting doctor: {Error}", ex.Message);
                return OperationResult<DoctorDto>.Failure("Database operation failed");
            }
        }
    }
}
