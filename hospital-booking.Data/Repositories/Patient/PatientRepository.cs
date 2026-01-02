using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Patient.Commands;
using hospital_booking.Data.Repositories.Patient.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.Patient
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ILogger<PatientRepository> _logger;

        public PatientRepository(ILogger<PatientRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreatePatientAsync(PatientAddDto dto)
        {
            return await CreatePatientCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<PatientDto>> UpdatePatientAsync(int patientId, PatientUpdateDto dto)
        {
            return await UpdatePatientCommand.ExecuteAsync(patientId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeletePatientAsync(int patientId)
        {
            return await DeletePatientCommand.ExecuteAsync(patientId, _logger);
        }

        public async Task<OperationResult<PatientDto>> GetPatientAsync(int patientId)
        {
            return await GetPatientQuery.ExecuteAsync(patientId, _logger);
        }

        public async Task<OperationResult<PatientsDto>> GetPatientsAsync(PatientsRequestDto requestDto)
        {
            return await GetPatientsQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
