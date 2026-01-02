using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Services.Interfaces
{
    public interface IClinicService
    {
        Task<OperationResult<ClinicDto>> GetClinicAsync(int clinicId);
        Task<OperationResult<ClinicsDto>> GetClinicsAsync(ClinicsRequestDto requestDto);
        Task<OperationResult<bool>> CreateClinicAsync(ClinicAddDto clinicDto);
        Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicUpdateDto clinicDto);
        Task<OperationResult<bool>> DeleteClinicAsync(int clinicId);
    Task<OperationResult<ClinicDetailsDto>> GetClinicDetailsAsync(int clinicId);
    }
}
