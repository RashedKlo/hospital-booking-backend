using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Prescription.Helpers;

namespace hospital_booking.Data.Repositories.Prescription.Queries
{
    public class GetPrescriptionsQuery
    {
        public static async Task<OperationResult<PrescriptionsDto>> ExecuteAsync(
            PrescriptionsRequestDto requestDto, 
            ILogger logger)
        {
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetPrescriptionsQuery received invalid params");
                return OperationResult<PrescriptionsDto>.Failure("Invalid parameters");
            }

            try 
            {
                var (sql, parameters) = BuildQuery(requestDto);

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }

                using var reader = await command.ExecuteReaderAsync();
                
                int totalCount = 0;
                if (await reader.ReadAsync())
                {
                    totalCount = reader.GetInt32(0);
                }

                await reader.NextResultAsync();

                var prescriptions = new List<PrescriptionDto>();
                while (await reader.ReadAsync())
                {
                    prescriptions.Add(PrescriptionMapper.MapFromReader(reader));
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var resultDto = new PrescriptionsDto
                {
                    Prescriptions = prescriptions,
                    Pagination = new PaginationDto
                    {
                        Page = requestDto.Page,
                        PageSize = requestDto.Limit,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasPrevious = requestDto.Page > 1,
                        HasNext = requestDto.Page < totalPages
                    }
                };

                return OperationResult<PrescriptionsDto>.Success(resultDto, "Prescriptions retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving prescriptions: {Error}", ex.Message);
                return OperationResult<PrescriptionsDto>.Failure("Database operation failed");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(PrescriptionsRequestDto request)
        {
            // No search/filters requested
            var parameters = new Dictionary<string, object>();

            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            var sql = $@"
-- Count
SELECT COUNT(*) FROM dbo.prescriptions;

-- Data
SELECT 
    p.prescription_id, p.appointment_id, p.instructions,
    a.appointment_id, a.patient_id, a.doctor_id, a.appointment_time, a.reason, a.status
FROM dbo.prescriptions p
INNER JOIN dbo.appointments a ON p.appointment_id = a.appointment_id
ORDER BY p.prescription_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return (sql, parameters);
        }
    }
}
