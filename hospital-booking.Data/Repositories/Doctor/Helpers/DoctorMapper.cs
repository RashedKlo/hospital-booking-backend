using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Helpers;
using Microsoft.Data.SqlClient;

namespace hospital_booking.Data.Repositories.Doctor.Helpers
{
    public static class DoctorMapper
    {
        public static Models.Doctor MapDoctorFromReader(SqlDataReader reader)
        {
            return new Models.Doctor
            {
                Id = reader.GetSafeInt32("id"),
                FullName = reader.GetSafeString("full_name"),
                Email = reader.GetSafeString("email"),
                Phone = reader.GetSafeString("phone"),
                PasswordHash = reader.GetSafeString("password_hash"),
                SpecialtyId = reader.GetSafeInt32("specialty_id"),
                ExperienceYears = reader.GetNullableInt32("experience_years"),
                Rating = reader.GetNullableDecimal("rating"),
                Bio = reader.GetSafeString("bio"),
                IsActive = reader.GetSafeBoolean("is_active"),
                CreatedAt = reader.GetSafeDateTime("created_at"),
                UpdatedAt = reader.GetNullableDateTime("updated_at")
            };
        }

        public static DoctorDto MapToDto(Models.Doctor doctor)
        {
            return new DoctorDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Email = doctor.Email,
                Phone = doctor.Phone,
                SpecialtyId = doctor.SpecialtyId,
                ExperienceYears = doctor.ExperienceYears,
                Rating = doctor.Rating,
                Bio = doctor.Bio,
                IsActive = doctor.IsActive,
                CreatedAt = doctor.CreatedAt,
                UpdatedAt = doctor.UpdatedAt
            };
        }
    }
}