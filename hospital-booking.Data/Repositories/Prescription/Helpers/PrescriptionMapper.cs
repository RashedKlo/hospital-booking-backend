using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.DTOs.Appointment;

namespace hospital_booking.Data.Repositories.Prescription.Helpers
{
    public static class PrescriptionMapper
    {
        public static PrescriptionDto MapFromReader(SqlDataReader reader)
        {
            var prescription = new PrescriptionDto
            {
                PrescriptionId = reader.GetInt32(0),
                AppointmentId = reader.GetInt32(1),
                Instructions = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
            };

            // Check for Appointment fields (starting index 3)
            if (reader.FieldCount > 3)
            {
                prescription.Appointment = new AppointmentDto
                {
                    AppointmentId = reader.GetInt32(3),
                    PatientId = reader.GetInt32(4),
                    DoctorId = reader.GetInt32(5),
                    AppointmentTime = reader.GetDateTime(6),
                    Reason = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Status = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                };
            }

            return prescription;
        }
    }
}
