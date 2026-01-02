using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.ClinicFacility.Commands;
using hospital_booking.Data.Repositories.ClinicFacility.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.ClinicFacility
{
    public class ClinicFacilityRepository : IClinicFacilityRepository
    {
        private readonly ILogger<ClinicFacilityRepository> _logger;

        public ClinicFacilityRepository(ILogger<ClinicFacilityRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateFacilityAsync(ClinicFacilityAddDto dto)
        {
            return await CreateFacilityCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<ClinicFacilityDto>> UpdateFacilityAsync(int facilityId, ClinicFacilityUpdateDto dto)
        {
            return await UpdateFacilityCommand.ExecuteAsync(facilityId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteFacilityAsync(int facilityId)
        {
            return await DeleteFacilityCommand.ExecuteAsync(facilityId, _logger);
        }

        public async Task<OperationResult<ClinicFacilityDto>> GetFacilityAsync(int facilityId)
        {
            return await GetFacilityQuery.ExecuteAsync(facilityId, _logger);
        }

        public async Task<OperationResult<List<ClinicFacilityDto>>> GetFacilitiesByClinicAsync(int clinicId)
        {
            return await GetFacilitiesQuery.ExecuteAsync(clinicId, _logger);
        }
    }
}
