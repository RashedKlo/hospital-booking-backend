namespace hospital_booking.Data.DTOs.Prescription
{
    public class PrescriptionsRequestDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        // No search or filter as requested
    }
}
