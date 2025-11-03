using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Helpers;
using Microsoft.Data.SqlClient;

namespace hospital_booking.Data.Repositories.Admin.Helpers
{
    public static class AdminMapper
    {
        public static Models.Admin MapAdminFromReader(SqlDataReader reader)
        {
            return new Models.Admin
            {
                Id = reader.GetSafeInt32("id"),
                FullName = reader.GetSafeString("full_name"),
                Email = reader.GetSafeString("email"),
                Phone = reader.GetSafeString("phone"),
                PasswordHash = reader.GetSafeString("password_hash"),
                Role = reader.GetSafeString("role"),
                IsActive = reader.GetSafeBoolean("is_active"),
                CreatedAt = reader.GetSafeDateTime("created_at"),
                UpdatedAt = reader.GetNullableDateTime("updated_at")
            };
        }

        public static AdminDto MapToDto(Models.Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                FullName = admin.FullName,
                Email = admin.Email,
                Phone = admin.Phone,
                Role = admin.Role,
                IsActive = admin.IsActive,
                CreatedAt = admin.CreatedAt,
                UpdatedAt = admin.UpdatedAt
            };
        }
    }
}