using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Doctor.Helpers;

namespace hospital_booking.Data.Repositories.Doctor.Queries
{
    public class GetDoctorsQuery
    {
        private const string GetSql = @"
SELECT
    doctor_id,
    clinic_id,
    full_name,
    bio,
    phone,
    is_active,
    experience_years
FROM dbo.doctors
ORDER BY doctor_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        public static async Task<OperationResult<List<DoctorDto>>> ExecuteAsync(int page, int limit, ILogger logger)
        {
            if (page <= 0 || limit <= 0)
            {
                logger.LogError("GetDoctorsQuery received invalid pagination: page={Page}, limit={Limit}", page, limit);
                return OperationResult<List<DoctorDto>>.Failure("Invalid pagination parameters");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                var offset = (page - 1) * limit;
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Limit", limit);

                using var reader = await command.ExecuteReaderAsync();
                var list = new List<DoctorDto>();
                while (await reader.ReadAsync())
                {
                    list.Add(DoctorMapper.MapFromReader(reader));
                }

                return OperationResult<List<DoctorDto>>.Success(list, "Doctors retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting doctors: {Error}", ex.Message);
                return OperationResult<List<DoctorDto>>.Failure("Database operation failed");
            }
        }
    }
}
