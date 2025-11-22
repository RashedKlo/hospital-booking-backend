using System;
using System.Collections.Generic;
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

        public async Task<OperationResult<PrescriptionItemDto>> CreatePrescriptionItemAsync(PrescriptionItemDto prescriptionItemDto)
        {
            return await CreatePrescriptionItemCommand.ExecuteAsync(prescriptionItemDto, _logger);
        }

        public async Task<OperationResult<PrescriptionItemDto>> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemDto prescriptionItemDto)
        {
            return await UpdatePrescriptionItemCommand.ExecuteAsync(itemId, prescriptionItemDto, _logger);
        }

        public async Task<OperationResult<bool>> DeletePrescriptionItemAsync(int itemId)
        {
            return await DeletePrescriptionItemCommand.ExecuteAsync(itemId, _logger);
        }

        public async Task<OperationResult<PrescriptionItemDto>> GetPrescriptionItemAsync(int itemId)
        {
            return await GetPrescriptionItemQuery.ExecuteAsync(itemId, _logger);
        }

        public async Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsAsync(int page, int limit)
        {
            return await GetPrescriptionItemsQuery.ExecuteAsync(page, limit, _logger);
        }

        public async Task<OperationResult<List<PrescriptionItemDto>>> GetPrescriptionItemsByPrescriptionAsync(int prescriptionId)
        {
            return await GetPrescriptionItemsByPrescriptionQuery.ExecuteAsync(prescriptionId, _logger);
        }
    }
}
