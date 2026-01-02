using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Patient.Helpers;

namespace hospital_booking.Data.Repositories.Patient.Queries
{
    public class GetPatientQuery
    {
        private const string GetSql = @"
SELECT 
    p.patient_id, p.user_id, p.full_name, p.birthDate, p.gender, p.notes,
    u.user_id, u.fullname, u.email, u.isGoogleLogin
FROM dbo.patients p
LEFT JOIN dbo.users u ON p.user_id = u.user_id
WHERE p.patient_id = @PatientId;
";

        public static async Task<OperationResult<PatientDto>> ExecuteAsync(int patientId, ILogger logger)
        {
            if (patientId <= 0)
            {
                return OperationResult<PatientDto>.Failure("Invalid patient ID");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return OperationResult<PatientDto>.Failure("Patient not found");
                }

                var dto = PatientMapper.MapFromReader(reader);
                return OperationResult<PatientDto>.Success(dto, "Patient retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting patient: {Error}", ex.Message);
                return OperationResult<PatientDto>.Failure("Database operation failed");
            }
        }
    }
}
