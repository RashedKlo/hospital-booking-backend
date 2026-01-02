using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Patient.Helpers;

namespace hospital_booking.Data.Repositories.Patient.Queries
{
    public class GetPatientsQuery
    {
        public static async Task<OperationResult<PatientsDto>> ExecuteAsync(
            PatientsRequestDto requestDto, 
            ILogger logger)
        {
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetPatientsQuery received invalid params");
                return OperationResult<PatientsDto>.Failure("Invalid parameters");
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

                var patients = new List<PatientDto>();
                while (await reader.ReadAsync())
                {
                    patients.Add(PatientMapper.MapFromReader(reader));
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var resultDto = new PatientsDto
                {
                    Patients = patients,
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

                return OperationResult<PatientsDto>.Success(resultDto, "Patients retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving patients: {Error}", ex.Message);
                return OperationResult<PatientsDto>.Failure("Database operation failed");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(PatientsRequestDto request)
        {
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var search = $"%{request.SearchQuery.Trim()}%";
                whereConditions.Add("(p.full_name LIKE @Search OR u.email LIKE @Search OR p.notes LIKE @Search)");
                parameters.Add("@Search", search);
            }

            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                whereConditions.Add("p.gender = @Gender");
                parameters.Add("@Gender", request.Gender);
            }

            var whereClause = whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            var sql = $@"
-- Count
SELECT COUNT(*) 
FROM dbo.patients p
LEFT JOIN dbo.users u ON p.user_id = u.user_id
{whereClause};

-- Data
SELECT 
    p.patient_id, p.user_id, p.full_name, p.birthDate, p.gender, p.notes,
    u.user_id, u.fullname, u.email, u.isGoogleLogin
FROM dbo.patients p
LEFT JOIN dbo.users u ON p.user_id = u.user_id
{whereClause}
ORDER BY p.patient_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return (sql, parameters);
        }
    }
}
