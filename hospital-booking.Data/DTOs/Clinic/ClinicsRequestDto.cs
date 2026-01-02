namespace hospital_booking.Data.DTOs.Clinic
{
    public class ClinicsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchQuery { get; set; }
        public double? MinRating { get; set; }
        public string? Address { get; set; }
    }
}
