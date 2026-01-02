using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Clinic.Helpers;

namespace hospital_booking.Data.Repositories.Clinic.Queries
{
    public class GetClinicQuery
    {
        private const string GetSql = @"
SELECT 
    clinic_id, name, description, address, phone, email, website, image_url, 
    rating, review_count, opening_hours, latitude, longitude, created_at, updated_at
FROM dbo.clinics
WHERE clinic_id = @ClinicId;
";

        public static async Task<OperationResult<ClinicDto>> ExecuteAsync(int clinicId, ILogger logger)
        {
            if (clinicId <= 0)
            {
                return OperationResult<ClinicDto>.Failure("Invalid clinic ID");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@ClinicId", clinicId);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<ClinicDto>.Failure("Clinic not found");
                }

                var dto = ClinicMapper.MapFromReader(reader);
                return OperationResult<ClinicDto>.Success(dto, "Clinic retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting clinic: {Error}", ex.Message);
                return OperationResult<ClinicDto>.Failure("Database operation failed");
            }
        }
    }
}
