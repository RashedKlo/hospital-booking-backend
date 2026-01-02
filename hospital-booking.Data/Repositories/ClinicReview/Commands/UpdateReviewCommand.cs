using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.ClinicReview.Queries;

namespace hospital_booking.Data.Repositories.ClinicReview.Commands
{
    public static class UpdateReviewCommand
    {
        public static async Task<OperationResult<ClinicReviewDto>> ExecuteAsync(int reviewId, ClinicReviewUpdateDto dto, ILogger logger)
        {
            try
            {
                var updates = new List<string>();
                var parameters = new List<SqlParameter>();

                if (dto.Rating.HasValue)
                {
                    updates.Add("rating = @Rating");
                    parameters.Add(new SqlParameter("@Rating", dto.Rating.Value));
                }

                if (dto.ReviewComment != null)
                {
                    updates.Add("review_comment = @ReviewComment");
                    parameters.Add(new SqlParameter("@ReviewComment", (object)dto.ReviewComment ?? DBNull.Value));
                }

                if (!updates.Any())
                {
                    return await GetReviewQuery.ExecuteAsync(reviewId, logger);
                }

                updates.Add("updated_at = GETDATE()");

                var sql = $@"
UPDATE dbo.clinic_reviews 
SET {string.Join(", ", updates)}
WHERE review_id = @ReviewId;

-- If rating was updated, recalculate clinic rating
IF EXISTS (SELECT 1 FROM @Params WHERE ParameterName = '@Rating')
BEGIN
    DECLARE @ClinicId INT = (SELECT clinic_id FROM dbo.clinic_reviews WHERE review_id = @ReviewId);
    UPDATE dbo.clinics 
    SET rating = (SELECT AVG(CAST(rating AS FLOAT)) FROM dbo.clinic_reviews WHERE clinic_id = @ClinicId)
    WHERE clinic_id = @ClinicId;
END
";
                // Note: The SQL above is simplified, but I'll write the logic more clearly for C# execution
                
                var updateSql = $@"
UPDATE dbo.clinic_reviews SET {string.Join(", ", updates)} WHERE review_id = @ReviewId;

DECLARE @ClinicId INT = (SELECT clinic_id FROM dbo.clinic_reviews WHERE review_id = @ReviewId);
UPDATE dbo.clinics SET rating = (SELECT AVG(CAST(rating AS FLOAT)) FROM dbo.clinic_reviews WHERE clinic_id = @ClinicId) WHERE clinic_id = @ClinicId;
";

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(updateSql, connection);
                command.Parameters.AddRange(parameters.ToArray());
                command.Parameters.AddWithValue("@ReviewId", reviewId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return await GetReviewQuery.ExecuteAsync(reviewId, logger);
                }
                return OperationResult<ClinicReviewDto>.Failure("Review not found or no changes made");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating review {ReviewId}: {Error}", reviewId, ex.Message);
                return OperationResult<ClinicReviewDto>.Failure("Database operation failed");
            }
        }
    }
}
