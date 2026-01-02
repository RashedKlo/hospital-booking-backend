using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.PrescriptionItem.Helpers;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Queries
{
    public class GetPrescriptionItemsQuery
    {
        public static async Task<OperationResult<PrescriptionItemsDto>> ExecuteAsync(
            PrescriptionItemsRequestDto requestDto, 
            ILogger logger)
        {
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetPrescriptionItemsQuery received invalid params");
                return OperationResult<PrescriptionItemsDto>.Failure("Invalid parameters");
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

                var items = new List<PrescriptionItemDto>();
                while (await reader.ReadAsync())
                {
                    items.Add(PrescriptionItemMapper.MapFromReader(reader));
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var resultDto = new PrescriptionItemsDto
                {
                    PrescriptionItems = items,
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

                return OperationResult<PrescriptionItemsDto>.Success(resultDto, "Prescription items retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving prescription items: {Error}", ex.Message);
                return OperationResult<PrescriptionItemsDto>.Failure("Database operation failed");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(PrescriptionItemsRequestDto request)
        {
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var search = $"%{request.SearchQuery.Trim()}%";
                whereConditions.Add("(pi.name LIKE @Search OR pi.dosage LIKE @Search OR pi.duration LIKE @Search OR pi.frequency LIKE @Search)");
                parameters.Add("@Search", search);
            }

            if (request.PrescriptionId.HasValue)
            {
                whereConditions.Add("pi.prescription_id = @PrescriptionId");
                parameters.Add("@PrescriptionId", request.PrescriptionId.Value);
            }

            var whereClause = whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            var sql = $@"
-- Count
SELECT COUNT(*) 
FROM dbo.prescription_items pi
INNER JOIN dbo.prescriptions p ON pi.prescription_id = p.prescription_id
{whereClause};

-- Data
SELECT 
    pi.item_id, pi.prescription_id, pi.name, pi.dosage, pi.duration, pi.frequency,
    p.prescription_id, p.appointment_id, p.instructions
FROM dbo.prescription_items pi
INNER JOIN dbo.prescriptions p ON pi.prescription_id = p.prescription_id
{whereClause}
ORDER BY pi.item_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return (sql, parameters);
        }
    }
}
