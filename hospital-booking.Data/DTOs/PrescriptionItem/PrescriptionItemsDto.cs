using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin;

namespace hospital_booking.Data.DTOs.PrescriptionItem
{
    public class PrescriptionItemsDto
    {
        public List<PrescriptionItemDto> PrescriptionItems { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
