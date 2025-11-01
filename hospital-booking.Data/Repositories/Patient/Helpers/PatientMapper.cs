using System;
using System.Data;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Helpers;
using Microsoft.Data.SqlClient;

namespace hospital_booking.Data.Repositories.Patient.Helpers
{
    public static class PatientMapper
    {
        public static Models.Patient MapPatientFromReader(SqlDataReader reader)
        {
            return new Models.Patient
            {
                Id = reader.GetSafeInt32("id"),
                FullName = reader.GetSafeString("full_name"),
                Email = reader.GetSafeString("email"),
                Phone = reader.GetSafeString("phone"),
                DateOfBirth = reader.GetSafeDateTime("date_of_birth"),
                PasswordHash = reader.GetSafeString("password_hash"),
                IsGoogleLogin = reader.GetSafeBoolean("is_google_login"),
                IsEmailVerified = reader.GetSafeBoolean("is_email_verified"),
                IsActive = reader.GetSafeBoolean("is_active"),
                SuspensionCount = reader.GetSafeInt32("suspension_count"),
                CreatedAt = reader.GetSafeDateTime("created_at"),
                UpdatedAt = reader.GetNullableDateTime("updated_at"),
                LastLogin = reader.GetNullableDateTime("last_login")
            };
        }

        public static PatientProfileDto MapToProfileDto(Models.Patient patient)
        {
            return new PatientProfileDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Email = patient.Email,
                Phone = patient.Phone,
                DateOfBirth = patient.DateOfBirth,
                IsGoogleLogin = patient.IsGoogleLogin,
                IsEmailVerified = patient.IsEmailVerified,
                IsActive = patient.IsActive,
                SuspensionCount = patient.SuspensionCount,
                CreatedAt = patient.CreatedAt,
                LastLogin = patient.LastLogin
            };
        }
    }
}