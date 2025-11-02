using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.Helpers;
using Microsoft.Data.SqlClient;

namespace hospital_booking.Data.Repositories.Specialty.Helpers
{
    public static class SpecialtyMapper
    {
        public static Models.Specialty MapSpecialtyFromReader(SqlDataReader reader)
        {
            return new Models.Specialty
            {
                Id = reader.GetSafeInt32("id"),
                DepartmentId = reader.GetSafeInt32("department_id"),
                Name = reader.GetSafeString("name"),
                Description = reader.GetSafeString("description"),
                IsActive = reader.GetSafeBoolean("is_active"),
                CreatedAt = reader.GetSafeDateTime("created_at"),
                UpdatedAt = reader.GetNullableDateTime("updated_at")
            };
        }

        public static SpecialtyDto MapToDto(Models.Specialty specialty)
        {
            return new SpecialtyDto
            {
                Id = specialty.Id,
                DepartmentId = specialty.DepartmentId,
                Name = specialty.Name,
                Description = specialty.Description,
                IsActive = specialty.IsActive,
                CreatedAt = specialty.CreatedAt,
                UpdatedAt = specialty.UpdatedAt
            };
        }
    }
}