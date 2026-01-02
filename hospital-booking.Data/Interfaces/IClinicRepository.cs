using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IClinicRepository
    {
        Task<OperationResult<ClinicDto>> GetClinicAsync(int clinicId);
        Task<OperationResult<ClinicsDto>> GetClinicsAsync(ClinicsRequestDto requestDto);
        Task<OperationResult<bool>> CreateClinicAsync(ClinicAddDto dto);
        Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicUpdateDto dto);
        Task<OperationResult<bool>> DeleteClinicAsync(int clinicId);
    }
}
