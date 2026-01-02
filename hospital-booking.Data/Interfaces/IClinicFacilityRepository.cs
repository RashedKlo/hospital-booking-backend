using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IClinicFacilityRepository
    {
        Task<OperationResult<ClinicFacilityDto>> GetFacilityAsync(int facilityId);
        Task<OperationResult<List<ClinicFacilityDto>>> GetFacilitiesByClinicAsync(int clinicId);
        Task<OperationResult<bool>> CreateFacilityAsync(ClinicFacilityAddDto dto);
        Task<OperationResult<ClinicFacilityDto>> UpdateFacilityAsync(int facilityId, ClinicFacilityUpdateDto dto);
        Task<OperationResult<bool>> DeleteFacilityAsync(int facilityId);
    }
}
