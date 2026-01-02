using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicReview.Queries
{
    public static class GetReviewsQuery
    {
        public static async Task<OperationResult<ClinicReviewsDto>> ExecuteAsync(ClinicReviewsRequestDto request, ILogger logger)
        {
            try
            {
                var reviews = new List<ClinicReviewDto>();
                var whereClauses = new List<string>();
                var parameters = new List<SqlParameter>();

                if (request.ClinicId.HasValue)
                {
                    whereClauses.Add("r.clinic_id = @ClinicId");
                    parameters.Add(new SqlParameter("@ClinicId", request.ClinicId.Value));
                }

                if (request.PatientId.HasValue)
                {
                    whereClauses.Add("r.patient_id = @PatientId");
                    parameters.Add(new SqlParameter("@PatientId", request.PatientId.Value));
                }

                if (request.MinRating.HasValue)
                {
                    whereClauses.Add("r.rating >= @MinRating");
                    parameters.Add(new SqlParameter("@MinRating", request.MinRating.Value));
                }

                var whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";
                
                var countSql = $"SELECT COUNT(*) FROM dbo.clinic_reviews r {whereSql}";
                
                var dataSql = $@"
SELECT r.review_id, r.clinic_id, r.patient_id, p.full_name as patient_name, 
       r.rating, r.review_comment, r.review_date, r.created_at
FROM dbo.clinic_reviews r
JOIN dbo.patients p ON r.patient_id = p.patient_id
{whereSql}
ORDER BY r.review_date DESC
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
                        reviews.Add(GetReviewQuery.MapToDto(reader));
                    }
                }

                return OperationResult<ClinicReviewsDto>.Success(new ClinicReviewsDto
                {
                    Reviews = reviews,
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
                logger.LogError(ex, "Error getting reviews: {Error}", ex.Message);
                return OperationResult<ClinicReviewsDto>.Failure("Database operation failed");
            }
        }
    }
}
