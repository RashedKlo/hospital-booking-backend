using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Admin;

namespace hospital_booking.Data.Repositories.Admin.Helpers
{
    public static class AdminMapper
    {
        public static AdminDto MapFromReader(SqlDataReader reader)
        {
            return new AdminDto
            {
                AdminId = reader.GetInt32(0),
                FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Role = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                IsActive = !reader.IsDBNull(5) && reader.GetBoolean(5),
                CreatedAt = reader.GetDateTime(6),
                UpdatedAt = reader.GetDateTime(7)
            };
        }
    }
}
