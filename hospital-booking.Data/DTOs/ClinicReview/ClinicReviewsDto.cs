using System.Collections.Generic;
using hospital_booking.Data.DTOs.Admin;

namespace hospital_booking.Data.DTOs.ClinicReview
{
    public class ClinicReviewsDto
    {
        public List<ClinicReviewDto> Reviews { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}
