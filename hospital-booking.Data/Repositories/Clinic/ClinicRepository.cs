using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Clinic.Commands;
using hospital_booking.Data.Repositories.Clinic.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.Clinic
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly ILogger<ClinicRepository> _logger;

        public ClinicRepository(ILogger<ClinicRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<ClinicDto>> CreateClinicAsync(ClinicDto clinicDto)
        {
            return await CreateClinicCommand.ExecuteAsync(clinicDto, _logger);
        }

        public async Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicDto clinicDto)
        {
            return await UpdateClinicCommand.ExecuteAsync(clinicId, clinicDto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteClinicAsync(int clinicId)
        {
            return await DeleteClinicCommand.ExecuteAsync(clinicId, _logger);
        }

        public async Task<OperationResult<ClinicDto>> GetClinicAsync(int clinicId)
        {
            return await GetClinicQuery.ExecuteAsync(clinicId, _logger);
        }

        public async Task<OperationResult<List<ClinicDto>>> GetClinicsAsync(int page, int limit)
        {
            return await GetClinicsQuery.ExecuteAsync(page, limit, _logger);
        }
    }
}
