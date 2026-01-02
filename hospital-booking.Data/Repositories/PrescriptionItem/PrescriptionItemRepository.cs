using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.PrescriptionItem.Commands;
using hospital_booking.Data.Repositories.PrescriptionItem.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.PrescriptionItem
{
    public class PrescriptionItemRepository : IPrescriptionItemRepository
    {
        private readonly ILogger<PrescriptionItemRepository> _logger;

        public PrescriptionItemRepository(ILogger<PrescriptionItemRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreatePrescriptionItemAsync(PrescriptionItemAddDto dto)
        {
            return await CreatePrescriptionItemCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<PrescriptionItemDto>> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemUpdateDto dto)
        {
            return await UpdatePrescriptionItemCommand.ExecuteAsync(itemId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeletePrescriptionItemAsync(int itemId)
        {
            return await DeletePrescriptionItemCommand.ExecuteAsync(itemId, _logger);
        }

        public async Task<OperationResult<PrescriptionItemDto>> GetPrescriptionItemAsync(int itemId)
        {
            return await GetPrescriptionItemQuery.ExecuteAsync(itemId, _logger);
        }

        public async Task<OperationResult<PrescriptionItemsDto>> GetPrescriptionItemsAsync(PrescriptionItemsRequestDto requestDto)
        {
            return await GetPrescriptionItemsQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
