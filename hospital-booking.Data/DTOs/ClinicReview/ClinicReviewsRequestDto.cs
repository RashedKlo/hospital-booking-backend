namespace hospital_booking.Data.DTOs.ClinicReview
{
    public class ClinicReviewsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int? ClinicId { get; set; }
        public int? PatientId { get; set; }
        public byte? MinRating { get; set; }
    }
}
