using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Prescription;

namespace hospital_booking.Data.Repositories.Prescription.Helpers
{
    public static class PrescriptionMapper
    {
        public static PrescriptionDto MapFromReader(SqlDataReader reader)
        {
            return new PrescriptionDto
            {
                PrescriptionId = reader.GetInt32(0),
                AppointmentId = reader.GetInt32(1),
                Instructions = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
            };
        }
    }
}
