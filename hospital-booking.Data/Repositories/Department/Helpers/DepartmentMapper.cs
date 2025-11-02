using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Helpers;
using Microsoft.Data.SqlClient;

namespace hospital_booking.Data.Repositories.Department.Helpers
{
    public static class DepartmentMapper
    {
        public static Models.Department MapDepartmentFromReader(SqlDataReader reader)
        {
            return new Models.Department
            {
                Id = reader.GetSafeInt32("id"),
                Name = reader.GetSafeString("name"),
                Description = reader.GetSafeString("description"),
                IsActive = reader.GetSafeBoolean("is_active"),
                CreatedAt = reader.GetSafeDateTime("created_at"),
                UpdatedAt = reader.GetNullableDateTime("updated_at")
            };
        }

        public static DepartmentDto MapToDto(Models.Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };
        }
    }
}