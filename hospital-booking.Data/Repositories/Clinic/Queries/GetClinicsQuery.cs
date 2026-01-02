using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.DTOs.Admin; // For PaginationDto
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Clinic.Helpers;

namespace hospital_booking.Data.Repositories.Clinic.Queries
{
    public class GetClinicsQuery
    {
        public static async Task<OperationResult<ClinicsDto>> ExecuteAsync(
            ClinicsRequestDto requestDto, 
            ILogger logger)
        {
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetClinicsQuery received invalid params");
                return OperationResult<ClinicsDto>.Failure("Invalid parameters");
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

                var clinics = new List<ClinicDto>();
                while (await reader.ReadAsync())
                {
                    clinics.Add(ClinicMapper.MapFromReader(reader));
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var resultDto = new ClinicsDto
                {
                    Clinics = clinics,
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

                return OperationResult<ClinicsDto>.Success(resultDto, "Clinics retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving clinics: {Error}", ex.Message);
                return OperationResult<ClinicsDto>.Failure("Database operation failed");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(ClinicsRequestDto request)
        {
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            // Search Logic
            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var search = $"%{request.SearchQuery.Trim()}%";
                whereConditions.Add("(name LIKE @Search OR description LIKE @Search OR address LIKE @Search)");
                parameters.Add("@Search", search);
            }

            // Specific Filters
            if (request.MinRating.HasValue)
            {
                whereConditions.Add("rating >= @MinRating");
                parameters.Add("@MinRating", request.MinRating.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                var addrFilter = $"%{request.Address.Trim()}%";
                whereConditions.Add("address LIKE @AddressFilter");
                parameters.Add("@AddressFilter", addrFilter);
            }

            var whereClause = whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            var sql = $@"
-- Count
SELECT COUNT(*) FROM dbo.clinics {whereClause};

-- Data
SELECT 
    clinic_id, name, description, address, phone, email, website, image_url, 
    rating, review_count, opening_hours, latitude, longitude, created_at, updated_at
FROM dbo.clinics
{whereClause}
ORDER BY clinic_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return (sql, parameters);
        }
    }
}
