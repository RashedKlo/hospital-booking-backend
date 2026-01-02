namespace hospital_booking.Data.DTOs.Doctor
{
    public class DoctorsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchQuery { get; set; }
        public int? ClinicId { get; set; }
        public int? MinExperienceYears { get; set; }
    }
}
