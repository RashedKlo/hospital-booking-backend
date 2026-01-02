using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Clinic.Commands
{
    public static class CreateClinicCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.clinics (
    name, description, address, phone, email, website, image_url, 
    rating, review_count, opening_hours, latitude, longitude
)
VALUES (
    @Name, @Description, @Address, @Phone, @Email, @Website, @ImageUrl, 
    0, 0, @OpeningHours, @Latitude, @Longitude
);
";

        public static async Task<OperationResult<bool>> ExecuteAsync(ClinicAddDto dto, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("CreateClinicCommand received null dto");
                return OperationResult<bool>.Failure("Clinic data is required");
            }

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(CreateSql, connection);
                command.Parameters.AddWithValue("@Name", dto.Name);
                command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", dto.Address);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@Email", (object?)dto.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@Website", (object?)dto.Website ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImageUrl", (object?)dto.ImageUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@OpeningHours", (object?)dto.OpeningHours ?? DBNull.Value);
                command.Parameters.AddWithValue("@Latitude", (object?)dto.Latitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@Longitude", (object?)dto.Longitude ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    return OperationResult<bool>.Success(true, "Clinic created successfully");
                }
                return OperationResult<bool>.Failure("Failed to create clinic");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating clinic: {Error}", ex.Message);
                return OperationResult<bool>.Failure("Database operation failed");
            }
        }
    }
}
