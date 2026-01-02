using System.Threading.Tasks;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IPrescriptionItemService
    {
        Task<OperationResult<PrescriptionItemDto>> GetPrescriptionItemAsync(int itemId);
        Task<OperationResult<PrescriptionItemsDto>> GetPrescriptionItemsAsync(PrescriptionItemsRequestDto requestDto);
        Task<OperationResult<bool>> CreatePrescriptionItemAsync(PrescriptionItemAddDto dto);
        Task<OperationResult<PrescriptionItemDto>> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemUpdateDto dto);
        Task<OperationResult<bool>> DeletePrescriptionItemAsync(int itemId);
    }
}
