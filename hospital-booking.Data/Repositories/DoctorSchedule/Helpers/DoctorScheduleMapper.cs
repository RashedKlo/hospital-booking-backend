using hospital_booking.Data.DTOs.DoctorSchedule;
using hospital_booking.Data.Helpers;
using Microsoft.Data.SqlClient;
using System;

namespace hospital_booking.Data.Repositories.DoctorSchedule.Helpers
{
    public static class DoctorScheduleMapper
    {
        public static Models.DoctorSchedule MapDoctorScheduleFromReader(SqlDataReader reader)
        {
            return new Models.DoctorSchedule
            {
                Id = reader.GetSafeInt32("id"),
                DoctorId = reader.GetSafeInt32("doctor_id"),
                DayOfWeek = reader.GetSafeString("day_of_week"),
                StartTime = TimeSpan.Parse(reader.GetSafeString("start_time")),
                EndTime = TimeSpan.Parse(reader.GetSafeString("end_time")),
                IsAvailable = reader.GetSafeBoolean("is_available"),
                BlockedReason = reader.GetSafeString("blocked_reason"),
                CreatedAt = reader.GetSafeDateTime("created_at"),
                UpdatedAt = reader.GetNullableDateTime("updated_at")
            };
        }

        public static DoctorScheduleDto MapToDto(Models.DoctorSchedule schedule)
        {
            return new DoctorScheduleDto
            {
                Id = schedule.Id,
                DoctorId = schedule.DoctorId,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                IsAvailable = schedule.IsAvailable,
                BlockedReason = schedule.BlockedReason,
                CreatedAt = schedule.CreatedAt,
                UpdatedAt = schedule.UpdatedAt
            };
        }
    }
}