using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IClinicService
    {
        Task<OperationResult<ClinicDto>> GetClinicAsync(int clinicId);
        Task<OperationResult<List<ClinicDto>>> GetClinicsAsync(int page, int limit);
        Task<OperationResult<ClinicDto>> CreateClinicAsync(ClinicDto clinicDto);
        Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicDto clinicDto);
        Task<OperationResult<bool>> DeleteClinicAsync(int clinicId);
    }
}
