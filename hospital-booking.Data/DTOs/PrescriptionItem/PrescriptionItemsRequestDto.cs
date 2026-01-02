namespace hospital_booking.Data.DTOs.PrescriptionItem
{
    public class PrescriptionItemsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SearchQuery { get; set; }
        public int? PrescriptionId { get; set; }
    }
}
