using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Models;
using hospital_booking.Data.Repositories.Patient.Commands;
using hospital_booking.Data.Repositories.Patient.Queries;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.Patient
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ILogger<PatientRepository> _logger;
        private readonly TokenHandler _tokenHandler;

        public PatientRepository(ILogger<PatientRepository> logger, TokenHandler tokenHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenHandler = tokenHandler ?? throw new ArgumentNullException(nameof(tokenHandler));
        }

        public async Task<OperationResult<PatientAuthenticationData>> RegisterPatientAsync(PatientRegistrationDto dto)
        {
            return await RegisterPatientCommand.ExecuteAsync(dto, _logger, _tokenHandler);
        }

        public async Task<OperationResult<PatientAuthenticationData>> LoginPatientAsync(PatientLoginDto dto)
        {
            return await LoginPatientCommand.ExecuteAsync(dto, _logger, _tokenHandler);
        }

        public async Task<OperationResult<PatientAuthenticationData>> GoogleLoginPatientAsync(PatientGoogleLoginDto dto)
        {
            // TODO: Implement Google OAuth logic
            throw new NotImplementedException("Google login not yet implemented");
        }

        public async Task<OperationResult<PatientProfileDto>> GetPatientByIdAsync(int patientId)
        {
            return await GetPatientByIdQuery.ExecuteAsync(patientId, _logger);
        }

        public async Task<OperationResult<PatientProfileDto>> GetPatientByEmailAsync(string email)
        {
            return await GetPatientByEmailQuery.ExecuteAsync(email, _logger);
        }

        public async Task<OperationResult<List<PatientProfileDto>>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 50)
        {
            return await GetAllPatientsQuery.ExecuteAsync(pageNumber, pageSize, _logger);
        }

        public async Task<OperationResult<PatientProfileDto>> UpdatePatientAsync(int patientId, PatientUpdateDto dto)
        {
            return await UpdatePatientCommand.ExecuteAsync(patientId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeletePatientAsync(int patientId)
        {
            return await DeletePatientCommand.ExecuteAsync(patientId, _logger);
        }

        public async Task<OperationResult<bool>> SuspendPatientAsync(int patientId)
        {
            // Increment suspension count
            throw new NotImplementedException("Suspend patient not yet implemented");
        }

        public async Task<OperationResult<bool>> ActivatePatientAsync(int patientId)
        {
            throw new NotImplementedException("Activate patient not yet implemented");
        }
    }
}