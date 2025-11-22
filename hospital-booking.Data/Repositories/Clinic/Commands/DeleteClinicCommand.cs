using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Clinic.Commands
{
    public static class DeleteClinicCommand
    {
        private const string DeleteClinicSql = @"
DELETE FROM dbo.clinics
WHERE clinic_id = @ClinicId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int clinicId,
            ILogger logger)
        {
            if (clinicId <= 0)
            {
                logger.LogError("DeleteClinicCommand received invalid clinic ID: {ClinicId}", clinicId);
                return OperationResult<bool>.Failure("Invalid clinic ID");
            }

            logger.LogInformation("Executing clinic deletion for ClinicId: {ClinicId}", clinicId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteClinicSql, connection);
                command.Parameters.AddWithValue("@ClinicId", clinicId);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Clinic not found for deletion - ClinicId: {ClinicId}", clinicId);
                    return OperationResult<bool>.Failure("Clinic not found");
                }

                logger.LogInformation("Clinic deleted successfully - ClinicId: {ClinicId}", clinicId);
                return OperationResult<bool>.Success(true, "Clinic deleted successfully");
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during clinic deletion for ClinicId: {ClinicId}. Error: {Error}",
                    clinicId, ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during clinic deletion for ClinicId: {ClinicId}", clinicId);
                return OperationResult<bool>.Failure("Clinic deletion failed due to system error");
            }
        }
    }
}
