using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicReview.Commands
{
    public static class DeleteReviewCommand
    {
        public static async Task<OperationResult<bool>> ExecuteAsync(int reviewId, ILogger logger)
        {
            try
            {
                var sql = @"
DECLARE @ClinicId INT = (SELECT clinic_id FROM dbo.clinic_reviews WHERE review_id = @ReviewId);

DELETE FROM dbo.clinic_reviews WHERE review_id = @ReviewId;

IF @ClinicId IS NOT NULL
BEGIN
    UPDATE dbo.clinics 
    SET rating = ISNULL((SELECT AVG(CAST(rating AS FLOAT)) FROM dbo.clinic_reviews WHERE clinic_id = @ClinicId), 0),
        review_count = (SELECT COUNT(*) FROM dbo.clinic_reviews WHERE clinic_id = @ClinicId)
    WHERE clinic_id = @ClinicId;
END
";

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ReviewId", reviewId);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Review deleted successfully");
                }
                return OperationResult<bool>.Failure("Review not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting review {ReviewId}: {Error}", reviewId, ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
