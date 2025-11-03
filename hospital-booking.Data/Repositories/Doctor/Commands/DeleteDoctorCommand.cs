using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Doctor.Commands
{
    public static class DeleteDoctorCommand
    {
        private const string DeleteDoctorSql = @"
            UPDATE doctors 
            SET is_active = 0, updated_at = GETDATE()
            WHERE id = @DoctorId AND is_active = 1";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int doctorId,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Deleting doctor: {Id}", doctorId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteDoctorSql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Doctor not found for deletion: {Id}", doctorId);
                    return OperationResult<bool>.Failure("Doctor not found");
                }

                logger.LogInformation("Doctor deleted successfully: {Id}", doctorId);
                return OperationResult<bool>.Success(true, "Doctor deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting doctor: {Id}", doctorId);
                return OperationResult<bool>.Failure("Deletion failed");
            }
        }
    }
}