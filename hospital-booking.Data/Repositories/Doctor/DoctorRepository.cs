using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Doctor.Commands;
using hospital_booking.Data.Repositories.Doctor.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.Doctor
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ILogger<DoctorRepository> _logger;

        public DoctorRepository(ILogger<DoctorRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<DoctorDto>> CreateDoctorAsync(DoctorDto dto)
        {
            return await CreateDoctorCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, DoctorDto dto)
        {
            return await UpdateDoctorCommand.ExecuteAsync(doctorId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId)
        {
            return await DeleteDoctorCommand.ExecuteAsync(doctorId, _logger);
        }

        public async Task<OperationResult<DoctorDto>> GetDoctorAsync(int doctorId)
        {
            return await GetDoctorQuery.ExecuteAsync(doctorId, _logger);
        }

        public async Task<OperationResult<List<DoctorDto>>> GetDoctorsAsync(int page, int limit)
        {
            return await GetDoctorsQuery.ExecuteAsync(page, limit, _logger);
        }
    }
}
