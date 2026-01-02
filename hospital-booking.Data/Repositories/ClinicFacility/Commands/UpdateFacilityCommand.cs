using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.ClinicFacility.Queries;

namespace hospital_booking.Data.Repositories.ClinicFacility.Commands
{
    public static class UpdateFacilityCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.clinic_facilities 
SET title = @Title, updated_at = GETDATE()
WHERE facility_id = @FacilityId;
";

        public static async Task<OperationResult<ClinicFacilityDto>> ExecuteAsync(int facilityId, ClinicFacilityUpdateDto dto, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@Title", dto.Title);
                command.Parameters.AddWithValue("@FacilityId", facilityId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return await GetFacilityQuery.ExecuteAsync(facilityId, logger);
                }
                return OperationResult<ClinicFacilityDto>.Failure("Facility not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating facility {FacilityId}: {Error}", facilityId, ex.Message);
                return OperationResult<ClinicFacilityDto>.Failure("Database operation failed");
            }
        }
    }
}
