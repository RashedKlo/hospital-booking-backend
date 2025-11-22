using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IPrescriptionItemService
    {
        Task<OperationResult<PrescriptionItemDto>> GetPrescriptionItemAsync(int itemId);
        Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsAsync(int page, int limit);
        Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsByPrescriptionAsync(int prescriptionId);
        Task<OperationResult<PrescriptionItemDto>> CreatePrescriptionItemAsync(PrescriptionItemDto prescriptionItemDto);
        Task<OperationResult<PrescriptionItemDto>> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemDto prescriptionItemDto);
        Task<OperationResult<bool>> DeletePrescriptionItemAsync(int itemId);
    }
}
