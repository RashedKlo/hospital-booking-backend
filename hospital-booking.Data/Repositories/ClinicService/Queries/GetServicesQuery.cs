using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicService.Queries
{
    public static class GetServicesQuery
    {
        public static async Task<OperationResult<ClinicServicesDto>> ExecuteAsync(ClinicServicesRequestDto request, ILogger logger)
        {
            try
            {
                var services = new List<ClinicServiceDto>();
                var whereClauses = new List<string>();
                var parameters = new List<SqlParameter>();

                if (request.ClinicId.HasValue)
                {
                    whereClauses.Add("clinic_id = @ClinicId");
                    parameters.Add(new SqlParameter("@ClinicId", request.ClinicId.Value));
                }

                if (!string.IsNullOrWhiteSpace(request.SearchQuery))
                {
                    whereClauses.Add("(title LIKE @Search OR description LIKE @Search)");
                    parameters.Add(new SqlParameter("@Search", $"%{request.SearchQuery}%"));
                }

                if (request.MinPrice.HasValue)
                {
                    whereClauses.Add("price >= @MinPrice");
                    parameters.Add(new SqlParameter("@MinPrice", request.MinPrice.Value));
                }

                if (request.MaxPrice.HasValue)
                {
                    whereClauses.Add("price <= @MaxPrice");
                    parameters.Add(new SqlParameter("@MaxPrice", request.MaxPrice.Value));
                }

                var whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";
                
                var countSql = $"SELECT COUNT(*) FROM dbo.clinic_services {whereSql}";
                
                var dataSql = $@"
SELECT service_id, clinic_id, title, description, price, created_at, updated_at
FROM dbo.clinic_services
{whereSql}
ORDER BY service_id
OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
";

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                // Get Total Count
                int totalCount;
                using (var countCmd = new SqlCommand(countSql, connection))
                {
                    countCmd.Parameters.AddRange(parameters.Select(p => (SqlParameter)((ICloneable)p).Clone()).ToArray());
                    totalCount = (int)(await countCmd.ExecuteScalarAsync() ?? 0);
                }

                // Get Data
                using (var dataCmd = new SqlCommand(dataSql, connection))
                {
                    dataCmd.Parameters.AddRange(parameters.Select(p => (SqlParameter)((ICloneable)p).Clone()).ToArray());
                    dataCmd.Parameters.AddWithValue("@Offset", (request.Page - 1) * request.Limit);
                    dataCmd.Parameters.AddWithValue("@Limit", request.Limit);

                    using var reader = await dataCmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        services.Add(GetServiceQuery.MapToDto(reader));
                    }
                }

                return OperationResult<ClinicServicesDto>.Success(new ClinicServicesDto
                {
                    Services = services,
                    Pagination = new PaginationDto
                    {
                        TotalItems = totalCount,
                        CurrentPage = request.Page,
                        PageSize = request.Limit,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)request.Limit)
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting services: {Error}", ex.Message);
                return OperationResult<ClinicServicesDto>.Failure("Database operation failed");
            }
        }
    }
}
