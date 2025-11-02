using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Department;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Department.Commands;
using hospital_booking.Data.Repositories.Department.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;

namespace hospital_booking.Data.Repositories.Department
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ILogger<DepartmentRepository> _logger;

        public DepartmentRepository(
            ILogger<DepartmentRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
          
        }

        public async Task<OperationResult<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            return await CreateDepartmentCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<DepartmentDto>> UpdateDepartmentAsync(int departmentId, UpdateDepartmentDto dto)
        {
            return await UpdateDepartmentCommand.ExecuteAsync(departmentId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteDepartmentAsync(int departmentId)
        {
            return await DeleteDepartmentCommand.ExecuteAsync(departmentId, _logger);
        }

        public async Task<OperationResult<DepartmentDto>> GetDepartmentByIdAsync(int departmentId)
        {
            return await GetDepartmentByIdQuery.ExecuteAsync(departmentId, _logger);
        }

        public async Task<OperationResult<List<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            return await GetAllDepartmentsQuery.ExecuteAsync(_logger);
        }
    }
}