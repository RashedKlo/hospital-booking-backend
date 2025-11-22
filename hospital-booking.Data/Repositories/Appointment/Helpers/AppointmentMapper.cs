using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Appointment;

namespace hospital_booking.Data.Repositories.Appointment.Helpers
{
    public static class AppointmentMapper
    {
        public static AppointmentDto MapFromReader(SqlDataReader reader)
        {
            return new AppointmentDto
            {
                AppointmentId = reader.GetInt32(0),
                PatientId = reader.GetInt32(1),
                DoctorId = reader.GetInt32(2),
                AppointmentTime = reader.GetDateTime(3),
                Reason = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Status = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            };
        }
    }
}
