using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<OperationResult<PrescriptionDto>> GetPrescriptionAsync(int prescriptionId);
        Task<OperationResult<PrescriptionsDto>> GetPrescriptionsAsync(PrescriptionsRequestDto requestDto);
        Task<OperationResult<bool>> CreatePrescriptionAsync(PrescriptionAddDto dto);
        Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionUpdateDto dto);
        Task<OperationResult<bool>> DeletePrescriptionAsync(int prescriptionId);
    }
}
