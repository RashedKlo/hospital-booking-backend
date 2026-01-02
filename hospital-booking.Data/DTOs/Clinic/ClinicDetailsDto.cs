using System;
using hospital_booking.Data.DTOs.ClinicReview;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.DTOs.ClinicService;

namespace hospital_booking.Data.DTOs.Clinic
{
    public class ClinicDetailsDto
    {
        public ClinicDto Clinic { get; set; } = new ClinicDto();
        public List<ClinicFacilityDto> Facilities { get; set; } = new List<ClinicFacilityDto>();
        public ClinicServicesDto Services { get; set; } = new ClinicServicesDto();   
        public ClinicReviewsDto Reviews { get; set; } = new ClinicReviewsDto();
        public DoctorsDto Doctors { get; set; } = new DoctorsDto();
    }
}