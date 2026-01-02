using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicFacility.Queries
{
    public static class GetFacilityQuery
    {
        private const string SelectSql = @"
SELECT facility_id, clinic_id, title, created_at, updated_at
FROM dbo.clinic_facilities
WHERE facility_id = @FacilityId;
";

        public static async Task<OperationResult<ClinicFacilityDto>> ExecuteAsync(int facilityId, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(SelectSql, connection);
                command.Parameters.AddWithValue("@FacilityId", facilityId);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var dto = MapToDto(reader);
                    return OperationResult<ClinicFacilityDto>.Success(dto);
                }
                return OperationResult<ClinicFacilityDto>.Failure("Facility not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting facility {FacilityId}: {Error}", facilityId, ex.Message);
                return OperationResult<ClinicFacilityDto>.Failure("Database operation failed");
            }
        }

        public static ClinicFacilityDto MapToDto(IDataReader reader)
        {
            return new ClinicFacilityDto
            {
                FacilityId = Convert.ToInt32(reader["facility_id"]),
                ClinicId = Convert.ToInt32(reader["clinic_id"]),
                Title = reader["title"].ToString() ?? string.Empty,
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };
        }
    }
}
