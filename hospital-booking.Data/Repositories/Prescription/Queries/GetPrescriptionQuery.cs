using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Prescription.Helpers;

namespace hospital_booking.Data.Repositories.Prescription.Queries
{
    public class GetPrescriptionQuery
    {
        private const string GetSql = @"
SELECT 
    p.prescription_id, p.appointment_id, p.instructions,
    a.appointment_id, a.patient_id, a.doctor_id, a.appointment_time, a.reason, a.status
FROM dbo.prescriptions p
INNER JOIN dbo.appointments a ON p.appointment_id = a.appointment_id
WHERE p.prescription_id = @PrescriptionId;
";

        public static async Task<OperationResult<PrescriptionDto>> ExecuteAsync(int prescriptionId, ILogger logger)
        {
            if (prescriptionId <= 0)
            {
                return OperationResult<PrescriptionDto>.Failure("Invalid prescription ID");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<PrescriptionDto>.Failure("Prescription not found");
                }

                var dto = PrescriptionMapper.MapFromReader(reader);
                return OperationResult<PrescriptionDto>.Success(dto, "Prescription retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting prescription: {Error}", ex.Message);
                return OperationResult<PrescriptionDto>.Failure("Database operation failed");
            }
        }
    }
}
