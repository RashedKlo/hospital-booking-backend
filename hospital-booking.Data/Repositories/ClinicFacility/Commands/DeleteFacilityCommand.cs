using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicFacility.Commands
{
    public static class DeleteFacilityCommand
    {
        private const string DeleteSql = "DELETE FROM dbo.clinic_facilities WHERE facility_id = @FacilityId;";

        public static async Task<OperationResult<bool>> ExecuteAsync(int facilityId, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteSql, connection);
                command.Parameters.AddWithValue("@FacilityId", facilityId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Facility deleted successfully");
                }
                return OperationResult<bool>.Failure("Facility not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting facility {FacilityId}: {Error}", facilityId, ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
