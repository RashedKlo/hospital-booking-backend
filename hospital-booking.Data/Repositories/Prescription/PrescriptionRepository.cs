using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Prescription.Commands;
using hospital_booking.Data.Repositories.Prescription.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.Prescription
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ILogger<PrescriptionRepository> _logger;

        public PrescriptionRepository(ILogger<PrescriptionRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreatePrescriptionAsync(PrescriptionAddDto dto)
        {
            return await CreatePrescriptionCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionUpdateDto dto)
        {
            return await UpdatePrescriptionCommand.ExecuteAsync(prescriptionId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeletePrescriptionAsync(int prescriptionId)
        {
            return await DeletePrescriptionCommand.ExecuteAsync(prescriptionId, _logger);
        }

        public async Task<OperationResult<PrescriptionDto>> GetPrescriptionAsync(int prescriptionId)
        {
            return await GetPrescriptionQuery.ExecuteAsync(prescriptionId, _logger);
        }

        public async Task<OperationResult<PrescriptionsDto>> GetPrescriptionsAsync(PrescriptionsRequestDto requestDto)
        {
            return await GetPrescriptionsQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
