namespace hospital_booking.Data.DTOs.MedicalReport
{
    public class MedicalReportsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchQuery { get; set; }
    }
}
