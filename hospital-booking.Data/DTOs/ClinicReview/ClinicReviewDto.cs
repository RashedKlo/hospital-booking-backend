using System;

namespace hospital_booking.Data.DTOs.ClinicReview
{
    public class ClinicReviewDto
    {
        public int ReviewId { get; set; }
        public int ClinicId { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; } // Joined from patients
        public byte Rating { get; set; }
        public string? ReviewComment { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
