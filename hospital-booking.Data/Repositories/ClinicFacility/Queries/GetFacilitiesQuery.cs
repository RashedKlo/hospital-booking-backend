using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicFacility.Queries
{
    public static class GetFacilitiesQuery
    {
        private const string SelectSql = @"
SELECT facility_id, clinic_id, title, created_at, updated_at
FROM dbo.clinic_facilities
WHERE clinic_id = @ClinicId
ORDER BY title;
";

        public static async Task<OperationResult<List<ClinicFacilityDto>>> ExecuteAsync(int clinicId, ILogger logger)
        {
            try
            {
                var facilities = new List<ClinicFacilityDto>();
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(SelectSql, connection);
                command.Parameters.AddWithValue("@ClinicId", clinicId);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    facilities.Add(GetFacilityQuery.MapToDto(reader));
                }

                return OperationResult<List<ClinicFacilityDto>>.Success(facilities);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting facilities for clinic {ClinicId}: {Error}", clinicId, ex.Message);
                return OperationResult<List<ClinicFacilityDto>>.Failure("Database operation failed");
            }
        }
    }
}
