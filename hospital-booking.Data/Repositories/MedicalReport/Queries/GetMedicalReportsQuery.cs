using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.MedicalReport.Helpers;

namespace hospital_booking.Data.Repositories.MedicalReport.Queries
{
    public class GetMedicalReportsQuery
    {
        public static async Task<OperationResult<MedicalReportsDto>> ExecuteAsync(
            MedicalReportsRequestDto requestDto, 
            ILogger logger)
        {
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetMedicalReportsQuery received invalid params");
                return OperationResult<MedicalReportsDto>.Failure("Invalid parameters");
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

                var reports = new List<MedicalReportDto>();
                while (await reader.ReadAsync())
                {
                    reports.Add(MedicalReportMapper.MapFromReader(reader));
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var resultDto = new MedicalReportsDto
                {
                    Reports = reports,
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

                return OperationResult<MedicalReportsDto>.Success(resultDto, "Medical reports retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving medical reports: {Error}", ex.Message);
                return OperationResult<MedicalReportsDto>.Failure("Database operation failed");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(MedicalReportsRequestDto request)
        {
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var search = $"%{request.SearchQuery.Trim()}%";
                whereConditions.Add("(mr.diagnosis LIKE @Search OR mr.notes LIKE @Search OR mr.required_tests LIKE @Search)");
                parameters.Add("@Search", search);
            }

            var whereClause = whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            var sql = $@"
-- Count
SELECT COUNT(*) 
FROM dbo.medical_reports mr
INNER JOIN dbo.appointments a ON mr.appointment_id = a.appointment_id
{whereClause};

-- Data
SELECT 
    mr.report_id, mr.appointment_id, mr.diagnosis, mr.notes, mr.required_tests,
    a.appointment_id, a.patient_id, a.doctor_id, a.appointment_time, a.reason, a.status
FROM dbo.medical_reports mr
INNER JOIN dbo.appointments a ON mr.appointment_id = a.appointment_id
{whereClause}
ORDER BY mr.report_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return (sql, parameters);
        }
    }
}
