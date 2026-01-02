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
SELECT 
    d.doctor_id, d.clinic_id, d.full_name, d.bio, d.phone, d.is_active, d.experience_years,
    c.clinic_id, c.name, c.description, c.address, c.phone, c.email, c.website, c.image_url, 
    c.rating, c.review_count, c.opening_hours, c.latitude, c.longitude, c.created_at, c.updated_at
FROM dbo.doctors d
INNER JOIN dbo.clinics c ON d.clinic_id = c.clinic_id
WHERE d.doctor_id = @DoctorId;
";

        public static async Task<OperationResult<DoctorDto>> ExecuteAsync(int doctorId, ILogger logger)
        {
            if (doctorId <= 0)
            {
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
