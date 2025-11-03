using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Models;
using hospital_booking.Data.Repositories.Doctor.Commands;
using hospital_booking.Data.Repositories.Doctor.Queries;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace hospital_booking.Data.Repositories.Doctor
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ILogger<DoctorRepository> _logger;
        private readonly TokenHandler _tokenHandler;
        private readonly string _connectionString;

        public DoctorRepository(
            ILogger<DoctorRepository> logger,
            TokenHandler tokenHandler,
            IOptions<DatabaseSettings> databaseSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenHandler = tokenHandler ?? throw new ArgumentNullException(nameof(tokenHandler));
            _connectionString = databaseSettings?.Value?.ConnectionString 
                ?? throw new ArgumentNullException(nameof(databaseSettings));
        }

        public async Task<OperationResult<DoctorAuthenticationData>> RegisterDoctorAsync(CreateDoctorDto dto)
        {
            return await RegisterDoctorCommand.ExecuteAsync(dto, _logger, _tokenHandler, _connectionString);
        }

        public async Task<OperationResult<DoctorAuthenticationData>> LoginDoctorAsync(DoctorLoginDto dto)
        {
            return await LoginDoctorCommand.ExecuteAsync(dto, _logger, _tokenHandler, _connectionString);
        }

        public async Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, UpdateDoctorDto dto)
        {
            return await UpdateDoctorCommand.ExecuteAsync(doctorId, dto, _logger, _connectionString);
        }

        public async Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId)
        {
            return await DeleteDoctorCommand.ExecuteAsync(doctorId, _logger, _connectionString);
        }

        public async Task<OperationResult<DoctorDto>> GetDoctorByIdAsync(int doctorId)
        {
            return await GetDoctorByIdQuery.ExecuteAsync(doctorId, _logger, _connectionString);
        }

        public async Task<OperationResult<List<DoctorDto>>> GetAllDoctorsAsync()
        {
            return await GetAllDoctorsQuery.ExecuteAsync(_logger, _connectionString);
        }

        public async Task<OperationResult<List<DoctorDto>>> GetDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await GetDoctorsBySpecialtyQuery.ExecuteAsync(specialtyId, _logger, _connectionString);
        }
    }
}