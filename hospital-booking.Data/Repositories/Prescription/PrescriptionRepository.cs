using System;
using System.Collections.Generic;
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

        public async Task<OperationResult<PrescriptionDto>> CreatePrescriptionAsync(PrescriptionDto prescriptionDto)
        {
            return await CreatePrescriptionCommand.ExecuteAsync(prescriptionDto, _logger);
        }

        public async Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionDto prescriptionDto)
        {
            return await UpdatePrescriptionCommand.ExecuteAsync(prescriptionId, prescriptionDto, _logger);
        }

        public async Task<OperationResult<bool>> DeletePrescriptionAsync(int prescriptionId)
        {
            return await DeletePrescriptionCommand.ExecuteAsync(prescriptionId, _logger);
        }

        public async Task<OperationResult<PrescriptionDto>> GetPrescriptionAsync(int prescriptionId)
        {
            return await GetPrescriptionQuery.ExecuteAsync(prescriptionId, _logger);
        }

        public async Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsAsync(int page, int limit)
        {
            return await GetPrescriptionsQuery.ExecuteAsync(page, limit, _logger);
        }

        public async Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsByAppointmentAsync(int appointmentId)
        {
            return await GetPrescriptionsByAppointmentQuery.ExecuteAsync(appointmentId, _logger);
        }
    }
}
