using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Interfaces
{
    public interface IPrescriptionItemRepository
    {
        /// <summary>
        /// Get a single prescription item by item ID
        /// </summary>
        Task<OperationResult<PrescriptionItemDto>> GetPrescriptionItemAsync(int itemId);

        /// <summary>
        /// Get all prescription items with pagination
        /// </summary>
        Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsAsync(int page, int limit);

        /// <summary>
        /// Get prescription items by prescription ID
        /// </summary>
        Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsByPrescriptionAsync(int prescriptionId);

        /// <summary>
        /// Create a new prescription item
        /// </summary>
        Task<OperationResult<PrescriptionItemDto>> CreatePrescriptionItemAsync(PrescriptionItemDto prescriptionItemDto);

        /// <summary>
        /// Update an existing prescription item
        /// </summary>
        Task<OperationResult<PrescriptionItemDto>> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemDto prescriptionItemDto);

        /// <summary>
        /// Delete a prescription item by item ID
        /// </summary>
        Task<OperationResult<bool>> DeletePrescriptionItemAsync(int itemId);
    }
}
