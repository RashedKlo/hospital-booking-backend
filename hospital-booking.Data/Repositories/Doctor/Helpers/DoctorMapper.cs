using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Doctor;

namespace hospital_booking.Data.Repositories.Doctor.Helpers
{
    public static class DoctorMapper
    {
        public static DoctorDto MapFromReader(SqlDataReader reader)
        {
            return new DoctorDto
            {
                DoctorId = reader.GetInt32(0),
                ClinicId = reader.GetInt32(1),
                FullName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Bio = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Phone = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                IsActive = !reader.IsDBNull(5) && reader.GetBoolean(5),
                ExperienceYears = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0
            };
        }
    }
}
