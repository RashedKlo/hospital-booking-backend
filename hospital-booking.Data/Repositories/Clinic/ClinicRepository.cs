using System;
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

        public async Task<OperationResult<bool>> CreateClinicAsync(ClinicAddDto dto)
        {
            return await CreateClinicCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicUpdateDto dto)
        {
            return await UpdateClinicCommand.ExecuteAsync(clinicId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteClinicAsync(int clinicId)
        {
            return await DeleteClinicCommand.ExecuteAsync(clinicId, _logger);
        }

        public async Task<OperationResult<ClinicDto>> GetClinicAsync(int clinicId)
        {
            return await GetClinicQuery.ExecuteAsync(clinicId, _logger);
        }

        public async Task<OperationResult<ClinicsDto>> GetClinicsAsync(ClinicsRequestDto requestDto)
        {
            return await GetClinicsQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
