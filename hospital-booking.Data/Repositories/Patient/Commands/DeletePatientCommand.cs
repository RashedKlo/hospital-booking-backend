using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Patient.Commands
{
    public static class DeletePatientCommand
    {
        private const string DeleteSql = @"
DELETE FROM dbo.patients
WHERE patient_id = @PatientId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(int patientId, ILogger logger)
        {
            if (patientId <= 0)
            {
                logger.LogError("DeletePatientCommand received invalid id: {PatientId}", patientId);
                return OperationResult<bool>.Failure("Invalid patient id");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteSql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<bool>.Failure("Patient not found");
                }

                return OperationResult<bool>.Success(true, "Patient deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting patient: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
