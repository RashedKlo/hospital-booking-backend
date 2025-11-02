using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Specialty.Commands
{
    public static class DeleteSpecialtyCommand
    {
        private const string DeleteSpecialtySql = @"
            UPDATE specialties 
            SET is_active = 0, updated_at = GETDATE()
            WHERE id = @SpecialtyId AND is_active = 1";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int specialtyId,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Deleting specialty: {Id}", specialtyId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteSpecialtySql, connection);
                command.Parameters.AddWithValue("@SpecialtyId", specialtyId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Specialty not found for deletion: {Id}", specialtyId);
                    return OperationResult<bool>.Failure("Specialty not found");
                }

                logger.LogInformation("Specialty deleted successfully: {Id}", specialtyId);
                return OperationResult<bool>.Success(true, "Specialty deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting specialty: {Id}", specialtyId);
                return OperationResult<bool>.Failure("Specialty deletion failed");
            }
        }
    }
}