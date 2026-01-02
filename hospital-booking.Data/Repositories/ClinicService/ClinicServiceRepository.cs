using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.ClinicService.Commands;
using hospital_booking.Data.Repositories.ClinicService.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.ClinicService
{
    public class ClinicServiceRepository : IClinicServiceRepository
    {
        private readonly ILogger<ClinicServiceRepository> _logger;

        public ClinicServiceRepository(ILogger<ClinicServiceRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateServiceAsync(ClinicServiceAddDto dto)
        {
            return await CreateServiceCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<ClinicServiceDto>> UpdateServiceAsync(int serviceId, ClinicServiceUpdateDto dto)
        {
            return await UpdateServiceCommand.ExecuteAsync(serviceId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteServiceAsync(int serviceId)
        {
            return await DeleteServiceCommand.ExecuteAsync(serviceId, _logger);
        }

        public async Task<OperationResult<ClinicServiceDto>> GetServiceAsync(int serviceId)
        {
            return await GetServiceQuery.ExecuteAsync(serviceId, _logger);
        }

        public async Task<OperationResult<ClinicServicesDto>> GetServicesAsync(ClinicServicesRequestDto requestDto)
        {
            return await GetServicesQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
