using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.ClinicService
{
    public class ClinicServicesService : IClinicServicesService
    {
        private readonly IClinicServiceRepository _clinicServiceRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<ClinicServicesService> _logger;

        public ClinicServicesService(
            IClinicServiceRepository clinicServiceRepository, 
            IClinicRepository clinicRepository,
            ILogger<ClinicServicesService> logger)
        {
            _clinicServiceRepository = clinicServiceRepository ?? throw new ArgumentNullException(nameof(clinicServiceRepository));
            _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateServiceAsync(ClinicServiceAddDto dto)
        {
            _logger.LogInformation("Creating service for ClinicId: {ClinicId}, Title: {Title}", 
                dto?.ClinicId, dto?.Title);

            var validationResult = await ClinicServiceValidation.ValidateAddAsync(dto!, _clinicRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            return await _clinicServiceRepository.CreateServiceAsync(dto!);
        }

        public async Task<OperationResult<ClinicServiceDto>> UpdateServiceAsync(int serviceId, ClinicServiceUpdateDto dto)
        {
            _logger.LogInformation("Updating service: {ServiceId}", serviceId);

            var validationResult = await ClinicServiceValidation.ValidateUpdateAsync(serviceId, dto, _clinicServiceRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<ClinicServiceDto>.Failure(validationResult.Message);
            }

            return await _clinicServiceRepository.UpdateServiceAsync(serviceId, dto);
        }


        public async Task<OperationResult<bool>> DeleteServiceAsync(int serviceId)
        {
            return await _clinicServiceRepository.DeleteServiceAsync(serviceId);
        }

        public async Task<OperationResult<ClinicServiceDto>> GetServiceAsync(int serviceId)
        {
            return await _clinicServiceRepository.GetServiceAsync(serviceId);
        }

        public async Task<OperationResult<ClinicServicesDto>> GetServicesAsync(ClinicServicesRequestDto requestDto)
        {
            return await _clinicServiceRepository.GetServicesAsync(requestDto);
        }
    }
}
