using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.DTOs.User;

namespace hospital_booking.Data.Repositories.Patient.Helpers
{
    public static class PatientMapper
    {
        public static PatientDto MapFromReader(SqlDataReader reader)
        {
            var patient = new PatientDto
            {
                PatientId = reader.GetInt32(0),
                UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                FullName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                BirthDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                Gender = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Notes = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            };

            // Check if User columns are present (start index 6)
            if (reader.FieldCount > 6)
            {
                patient.User = new UserDto
                {
                    UserId = reader.GetInt32(6),
                    FullName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Email = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    IsGoogleLogin = reader.IsDBNull(9) ? false : reader.GetBoolean(9)
                };
            }

            return patient;
        }
    }
}
