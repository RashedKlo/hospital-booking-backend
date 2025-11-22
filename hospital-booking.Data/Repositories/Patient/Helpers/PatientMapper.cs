using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Patient;

namespace hospital_booking.Data.Repositories.Patient.Helpers
{
    public static class PatientMapper
    {
        public static PatientDto MapFromReader(SqlDataReader reader)
        {
            return new PatientDto
            {
                PatientId = reader.GetInt32(0),
                UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                FullName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                BirthDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                Gender = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Notes = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            };
        }
    }
}
