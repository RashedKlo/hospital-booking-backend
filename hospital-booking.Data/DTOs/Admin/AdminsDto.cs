namespace hospital_booking.Data.DTOs.Admin;
    public class AdminsDto
    {
        public PaginationDto Pagination { get; set; } = new();
        public List<AdminDto> Admins { get; set; } = new();
    }
