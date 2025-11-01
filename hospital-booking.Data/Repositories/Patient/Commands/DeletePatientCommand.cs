using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class DeletePatientCommand
    {
        private const string DeletePatientSql = @"
            UPDATE patients 
            SET is_active = 0, updated_at = GETDATE()
            WHERE id = @PatientId AND is_active = 1";

        public static async Task<OperationResult<bool>> ExecuteAsync(int patientId, ILogger logger)
        {
            logger.LogInformation("Deleting (deactivating) patient: {PatientId}", patientId);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeletePatientSql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Patient not found for deletion: {PatientId}", patientId);
                    return OperationResult<bool>.Failure("Patient not found");
                }

                logger.LogInformation("Patient deleted successfully: {PatientId}", patientId);
                return OperationResult<bool>.Success(true, "Patient deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting patient: {PatientId}", patientId);
                return OperationResult<bool>.Failure("Delete failed due to system error");
            }
        }
    }
}