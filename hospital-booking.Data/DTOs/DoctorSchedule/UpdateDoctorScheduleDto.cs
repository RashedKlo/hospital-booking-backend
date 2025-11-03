using System;
using System.ComponentModel.DataAnnotations;

namespace hospital_booking.Data.DTOs.DoctorSchedule
{
    public class UpdateDoctorScheduleDto
    {
        [Required(ErrorMessage = "Start time is required")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        [StringLength(500, ErrorMessage = "Blocked reason cannot exceed 500 characters")]
        public string? BlockedReason { get; set; }
    }
}