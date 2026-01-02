using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicReview.Queries
{
    public static class GetReviewQuery
    {
        private const string SelectSql = @"
SELECT r.review_id, r.clinic_id, r.patient_id, p.full_name as patient_name, 
       r.rating, r.review_comment, r.review_date, r.created_at
FROM dbo.clinic_reviews r
JOIN dbo.patients p ON r.patient_id = p.patient_id
WHERE r.review_id = @ReviewId;
";

        public static async Task<OperationResult<ClinicReviewDto>> ExecuteAsync(int reviewId, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(SelectSql, connection);
                command.Parameters.AddWithValue("@ReviewId", reviewId);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var dto = MapToDto(reader);
                    return OperationResult<ClinicReviewDto>.Success(dto);
                }
                return OperationResult<ClinicReviewDto>.Failure("Review not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting review {ReviewId}: {Error}", reviewId, ex.Message);
                return OperationResult<ClinicReviewDto>.Failure("Database operation failed");
            }
        }

        public static ClinicReviewDto MapToDto(IDataReader reader)
        {
            return new ClinicReviewDto
            {
                ReviewId = Convert.ToInt32(reader["review_id"]),
                ClinicId = Convert.ToInt32(reader["clinic_id"]),
                PatientId = Convert.ToInt32(reader["patient_id"]),
                PatientName = reader["patient_name"].ToString(),
                Rating = Convert.ToByte(reader["rating"]),
                ReviewComment = reader["review_comment"]?.ToString(),
                ReviewDate = Convert.ToDateTime(reader["review_date"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
}
