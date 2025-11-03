using System;

namespace hospital_booking.Data.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty; // sunday, monday, etc.
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? BlockedReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}