using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Patient.Helpers;

namespace hospital_booking.Data.Repositories.Patient.Queries
{
    public class GetPatientsQuery
    {
        private const string GetSql = @"
SELECT
    patient_id,
    user_id,
    full_name,
    birthDate,
    gender,
    notes
FROM dbo.patients
ORDER BY patient_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";

        public static async Task<OperationResult<List<PatientDto>>> ExecuteAsync(int page, int limit, ILogger logger)
        {
            if (page <= 0 || limit <= 0)
            {
                logger.LogError("GetPatientsQuery received invalid pagination");
                return OperationResult<List<PatientDto>>.Failure("Invalid pagination");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetSql, connection);
                var offset = (page - 1) * limit;
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Limit", limit);

                using var reader = await command.ExecuteReaderAsync();
                var list = new List<PatientDto>();
                while (await reader.ReadAsync())
                {
                    list.Add(PatientMapper.MapFromReader(reader));
                }

                return OperationResult<List<PatientDto>>.Success(list, "Patients retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting patients: {Error}", ex.Message);
                return OperationResult<List<PatientDto>>.Failure("Database operation failed");
            }
        }
    }
}
