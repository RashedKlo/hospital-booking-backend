using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.ClinicReview.Commands
{
    public static class CreateReviewCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.clinic_reviews (
    clinic_id, patient_id, rating, review_comment, review_date, created_at, updated_at
)
VALUES (
    @ClinicId, @PatientId, @Rating, @ReviewComment, GETDATE(), GETDATE(), GETDATE()
);

-- Update clinic rating and review count
UPDATE dbo.clinics 
SET rating = (SELECT AVG(CAST(rating AS FLOAT)) FROM dbo.clinic_reviews WHERE clinic_id = @ClinicId),
    review_count = (SELECT COUNT(*) FROM dbo.clinic_reviews WHERE clinic_id = @ClinicId)
WHERE clinic_id = @ClinicId;
";

        public static async Task<OperationResult<bool>> ExecuteAsync(ClinicReviewAddDto dto, ILogger logger)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@ClinicId", dto.ClinicId);
                command.Parameters.AddWithValue("@PatientId", dto.PatientId);
                command.Parameters.AddWithValue("@Rating", dto.Rating);
                command.Parameters.AddWithValue("@ReviewComment", (object?)dto.ReviewComment ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Review submitted successfully");
                }
                return OperationResult<bool>.Failure("Failed to submit review");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating review: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
