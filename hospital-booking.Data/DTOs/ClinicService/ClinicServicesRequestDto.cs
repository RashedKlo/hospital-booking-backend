namespace hospital_booking.Data.DTOs.ClinicService
{
    public class ClinicServicesRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int? ClinicId { get; set; }
        public string? SearchQuery { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
