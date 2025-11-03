using System;
using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.DoctorSchedule
{
    public class CreateDoctorScheduleDto
    {
        [Required(ErrorMessage = "Doctor ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Day of week is required")]
        [RegularExpression("^(sunday|monday|tuesday|wednesday|thursday|friday|saturday)$", 
            ErrorMessage = "Day of week must be sunday, monday, tuesday, wednesday, thursday, friday, or saturday")]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        [StringLength(500, ErrorMessage = "Blocked reason cannot exceed 500 characters")]
        public string? BlockedReason { get; set; }
    }
}