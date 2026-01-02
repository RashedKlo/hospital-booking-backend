using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Clinic.Queries;

namespace hospital_booking.Data.Repositories.Clinic.Commands
{
    public static class UpdateClinicCommand
    {
        private const string UpdateSql = @"
UPDATE dbo.clinics
SET name = ISNULL(@Name, name),
    description = ISNULL(@Description, description),
    address = ISNULL(@Address, address),
    phone = ISNULL(@Phone, phone),
    email = ISNULL(@Email, email),
    website = ISNULL(@Website, website),
    image_url = ISNULL(@ImageUrl, image_url),
    opening_hours = ISNULL(@OpeningHours, opening_hours),
    latitude = ISNULL(@Latitude, latitude),
    longitude = ISNULL(@Longitude, longitude),
    updated_at = GETDATE()
WHERE clinic_id = @ClinicId;
";

        public static async Task<OperationResult<ClinicDto>> ExecuteAsync(int clinicId, ClinicUpdateDto dto, ILogger logger)
        {
            if (clinicId <= 0) return OperationResult<ClinicDto>.Failure("Invalid clinic ID");
            if (dto == null) return OperationResult<ClinicDto>.Failure("Data is required");

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(UpdateSql, connection);
                command.Parameters.AddWithValue("@ClinicId", clinicId);
                command.Parameters.AddWithValue("@Name", (object?)dto.Name ?? DBNull.Value);
                command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object?)dto.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@Phone", (object?)dto.Phone ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object?)dto.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@Website", (object?)dto.Website ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImageUrl", (object?)dto.ImageUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@OpeningHours", (object?)dto.OpeningHours ?? DBNull.Value);
                command.Parameters.AddWithValue("@Latitude", (object?)dto.Latitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@Longitude", (object?)dto.Longitude ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    return OperationResult<ClinicDto>.Failure("Clinic not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating clinic: {Error}", ex.Message);
                return OperationResult<ClinicDto>.Failure("Database operation failed");
            }

            return await GetClinicQuery.ExecuteAsync(clinicId, logger);
        }
    }
}
