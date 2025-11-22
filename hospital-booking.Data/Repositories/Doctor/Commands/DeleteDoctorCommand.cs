using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Doctor.Commands
{
    public static class DeleteDoctorCommand
    {
        private const string DeleteSql = @"
DELETE FROM dbo.doctors
WHERE doctor_id = @DoctorId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(int doctorId, ILogger logger)
        {
            if (doctorId <= 0)
            {
                logger.LogError("DeleteDoctorCommand received invalid id: {DoctorId}", doctorId);
                return OperationResult<bool>.Failure("Invalid doctor id");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(DeleteSql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<bool>.Failure("Doctor not found");
                }

                return OperationResult<bool>.Success(true, "Doctor deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting doctor: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
