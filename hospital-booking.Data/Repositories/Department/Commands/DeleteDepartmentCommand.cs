using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Department.Commands
{
    public static class DeleteDepartmentCommand
    {
        private const string DeleteDepartmentSql = @"
            UPDATE departments 
            SET is_active = 0, updated_at = GETDATE()
            WHERE id = @DepartmentId AND is_active = 1";

        public static async Task<OperationResult<bool>> ExecuteAsync(
            int departmentId,
            ILogger logger,
            string connectionString)
        {
            logger.LogInformation("Deleting department: {Id}", departmentId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteDepartmentSql, connection);
                command.Parameters.AddWithValue("@DepartmentId", departmentId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    logger.LogWarning("Department not found for deletion: {Id}", departmentId);
                    return OperationResult<bool>.Failure("Department not found");
                }

                logger.LogInformation("Department deleted successfully: {Id}", departmentId);
                return OperationResult<bool>.Success(true, "Department deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting department: {Id}", departmentId);
                return OperationResult<bool>.Failure("Department deletion failed");
            }
        }
    }
}