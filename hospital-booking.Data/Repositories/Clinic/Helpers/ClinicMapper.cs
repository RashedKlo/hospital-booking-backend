using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Clinic;

namespace hospital_booking.Data.Repositories.Clinic.Helpers
{
    public static class ClinicMapper
    {
        public static ClinicDto MapFromReader(SqlDataReader reader)
        {
            return new ClinicDto
            {
                ClinicId = reader.GetInt32(0),
                Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Address = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Phone = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                Website = reader.IsDBNull(6) ? null : reader.GetString(6),
                ImageUrl = reader.IsDBNull(7) ? null : reader.GetString(7),
                Rating = reader.IsDBNull(8) ? (double?)null : reader.GetDouble(8),
                ReviewCount = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                OpeningHours = reader.IsDBNull(10) ? null : reader.GetString(10),
                Latitude = reader.IsDBNull(11) ? (double?)null : reader.GetDouble(11),
                Longitude = reader.IsDBNull(12) ? (double?)null : reader.GetDouble(12),
                CreatedAt = reader.IsDBNull(13) ? DateTime.MinValue : reader.GetDateTime(13),
                UpdatedAt = reader.IsDBNull(14) ? DateTime.MinValue : reader.GetDateTime(14)
            };
        }
    }
}
