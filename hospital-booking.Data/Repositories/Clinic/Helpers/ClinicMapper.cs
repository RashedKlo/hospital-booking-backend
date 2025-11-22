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
                Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Phone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Address = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            };
        }
    }
}
