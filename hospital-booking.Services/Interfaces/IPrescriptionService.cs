using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<OperationResult<PrescriptionDto>> GetPrescriptionAsync(int prescriptionId);
        Task<OperationResult<PrescriptionsDto>> GetPrescriptionsAsync(PrescriptionsRequestDto requestDto);
        Task<OperationResult<bool>> CreatePrescriptionAsync(PrescriptionAddDto dto);
        Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionUpdateDto dto);
        Task<OperationResult<bool>> DeletePrescriptionAsync(int prescriptionId);
    }
}
