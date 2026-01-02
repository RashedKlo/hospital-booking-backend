using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.DTOs.Doctor;

namespace hospital_booking.Data.Repositories.Appointment.Helpers
{
    public static class AppointmentMapper
    {
        public static AppointmentDto MapFromReader(SqlDataReader reader)
        {
            var appointment = new AppointmentDto
            {
                AppointmentId = reader.GetInt32(0),
                PatientId = reader.GetInt32(1),
                DoctorId = reader.GetInt32(2),
                AppointmentTime = reader.GetDateTime(3),
                Reason = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Status = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
            };

            // Map Patient (indices 6-10)
            appointment.Patient = new PatientDto
            {
                PatientId = reader.GetInt32(6),
                // UserId skipped or null in this context as not joined/selected typically unless requested, assume Patient table cols
                FullName = reader.GetString(7),
                BirthDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                Gender = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                Notes = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
            };

            // Map Doctor (indices 11-17)
            appointment.Doctor = new DoctorDto
            {
                DoctorId = reader.GetInt32(11),
                ClinicId = reader.GetInt32(12),
                FullName = reader.GetString(13),
                Bio = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                Phone = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                IsActive = reader.GetBoolean(16),
                ExperienceYears = reader.GetInt32(17)
            };

            return appointment;
        }
    }
}
