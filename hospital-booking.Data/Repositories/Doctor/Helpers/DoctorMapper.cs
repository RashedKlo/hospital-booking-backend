using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.DTOs.Clinic;

namespace hospital_booking.Data.Repositories.Doctor.Helpers
{
    public static class DoctorMapper
    {
        public static DoctorDto MapFromReader(SqlDataReader reader)
        {
            var doctor = new DoctorDto
            {
                DoctorId = reader.GetInt32(0),
                ClinicId = reader.GetInt32(1),
                FullName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Bio = reader.IsDBNull(3) ? null : reader.GetString(3),
                Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                IsActive = !reader.IsDBNull(5) && reader.GetBoolean(5),
                ExperienceYears = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0
            };

            // Check if we have clinic data joined (simplest check is column count, but let's assume query structure)
            // If the query includes clinic columns, they start at index 7
            if (reader.FieldCount > 7)
            {
                doctor.Clinic = new ClinicDto
                {
                    ClinicId = reader.GetInt32(7),
                    Name = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    Description = reader.IsDBNull(9) ? null : reader.GetString(9),
                    Address = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                    Phone = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                    Email = reader.IsDBNull(12) ? null : reader.GetString(12),
                    Website = reader.IsDBNull(13) ? null : reader.GetString(13),
                    ImageUrl = reader.IsDBNull(14) ? null : reader.GetString(14),
                    Rating = reader.IsDBNull(15) ? (double?)null : reader.GetDouble(15),
                    ReviewCount = reader.IsDBNull(16) ? (int?)null : reader.GetInt32(16),
                    OpeningHours = reader.IsDBNull(17) ? null : reader.GetString(17),
                    Latitude = reader.IsDBNull(18) ? (double?)null : reader.GetDouble(18),
                    Longitude = reader.IsDBNull(19) ? (double?)null : reader.GetDouble(19),
                    CreatedAt = reader.IsDBNull(20) ? DateTime.MinValue : reader.GetDateTime(20),
                    UpdatedAt = reader.IsDBNull(21) ? DateTime.MinValue : reader.GetDateTime(21)
                };
            }

            return doctor;
        }
    }
}
