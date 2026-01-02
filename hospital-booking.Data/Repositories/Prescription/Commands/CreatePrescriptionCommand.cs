using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Prescription.Commands
{
    public static class CreatePrescriptionCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.prescriptions (appointment_id, instructions)
VALUES (@AppointmentId, @Instructions);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(PrescriptionAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreatePrescriptionCommand received null dto");
                return OperationResult<bool>.Failure("Prescription data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@AppointmentId", dto.AppointmentId);
                command.Parameters.AddWithValue("@Instructions", (object?)dto.Instructions ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Prescription created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create prescription");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating prescription: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
