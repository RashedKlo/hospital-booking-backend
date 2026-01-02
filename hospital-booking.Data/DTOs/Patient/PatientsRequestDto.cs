namespace hospital_booking.Data.DTOs.Patient
{
    public class PatientsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchQuery { get; set; }
        public string? Gender { get; set; }
    }
}
