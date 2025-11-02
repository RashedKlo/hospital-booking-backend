using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Specialty;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Specialty.Commands;
using hospital_booking.Data.Repositories.Specialty.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Specialty
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly ILogger<SpecialtyRepository> _logger;

        public SpecialtyRepository(
            ILogger<SpecialtyRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           
        }

        public async Task<OperationResult<SpecialtyDto>> CreateSpecialtyAsync(CreateSpecialtyDto dto)
        {
            return await CreateSpecialtyCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<SpecialtyDto>> UpdateSpecialtyAsync(int specialtyId, UpdateSpecialtyDto dto)
        {
            return await UpdateSpecialtyCommand.ExecuteAsync(specialtyId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteSpecialtyAsync(int specialtyId)
        {
            return await DeleteSpecialtyCommand.ExecuteAsync(specialtyId, _logger);
        }

        public async Task<OperationResult<SpecialtyDto>> GetSpecialtyByIdAsync(int specialtyId)
        {
            return await GetSpecialtyByIdQuery.ExecuteAsync(specialtyId, _logger);
        }

        public async Task<OperationResult<List<SpecialtyDto>>> GetAllSpecialtiesAsync()
        {
            return await GetAllSpecialtiesQuery.ExecuteAsync(_logger);
        }

        public async Task<OperationResult<List<SpecialtyDto>>> GetSpecialtiesByDepartmentAsync(int departmentId)
        {
            return await GetSpecialtiesByDepartmentQuery.ExecuteAsync(departmentId, _logger);
        }
    }
}