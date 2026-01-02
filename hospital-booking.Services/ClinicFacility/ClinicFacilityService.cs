using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicFacility;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.ClinicFacility
{
    public class ClinicFacilityService : IClinicFacilityService
    {
        private readonly IClinicFacilityRepository _clinicFacilityRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<ClinicFacilityService> _logger;

        public ClinicFacilityService(
            IClinicFacilityRepository clinicFacilityRepository, 
            IClinicRepository clinicRepository,
            ILogger<ClinicFacilityService> logger)
        {
            _clinicFacilityRepository = clinicFacilityRepository ?? throw new ArgumentNullException(nameof(clinicFacilityRepository));
            _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateFacilityAsync(ClinicFacilityAddDto dto)
        {
            _logger.LogInformation("Creating facility for ClinicId: {ClinicId}, Title: {Title}", 
                dto?.ClinicId, dto?.Title);

            var validationResult = await ClinicFacilityValidation.ValidateAddAsync(dto!, _clinicRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            return await _clinicFacilityRepository.CreateFacilityAsync(dto!);
        }

        public async Task<OperationResult<ClinicFacilityDto>> UpdateFacilityAsync(int facilityId, ClinicFacilityUpdateDto dto)
        {
            _logger.LogInformation("Updating facility: {FacilityId}", facilityId);

            var validationResult = await ClinicFacilityValidation.ValidateUpdateAsync(facilityId, dto, _clinicFacilityRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<ClinicFacilityDto>.Failure(validationResult.Message);
            }

            return await _clinicFacilityRepository.UpdateFacilityAsync(facilityId, dto);
        }


        public async Task<OperationResult<bool>> DeleteFacilityAsync(int facilityId)
        {
            return await _clinicFacilityRepository.DeleteFacilityAsync(facilityId);
        }

        public async Task<OperationResult<ClinicFacilityDto>> GetFacilityAsync(int facilityId)
        {
            return await _clinicFacilityRepository.GetFacilityAsync(facilityId);
        }

        public async Task<OperationResult<List<ClinicFacilityDto>>> GetFacilitiesByClinicAsync(int clinicId)
        {
            return await _clinicFacilityRepository.GetFacilitiesByClinicAsync(clinicId);
        }
    }
}
