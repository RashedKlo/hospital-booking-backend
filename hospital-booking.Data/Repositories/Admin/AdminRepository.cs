using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.Admin.Commands;
using hospital_booking.Data.Repositories.Admin.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ILogger<AdminRepository> _logger;

        public AdminRepository(ILogger<AdminRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<AdminDto>> CreateAdminAsync(AdminAddDto dto)
        {
            return await CreateAdminCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminUpdateDto dto)
        {
            return await UpdateAdminCommand.ExecuteAsync(adminId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteAdminAsync(int adminId)
        {
            return await DeleteAdminCommand.ExecuteAsync(adminId, _logger);
        }

        public async Task<OperationResult<AdminDto>> GetAdminAsync(int adminId)
        {
            return await GetAdminQuery.ExecuteAsync(adminId, _logger);
        }

        public async Task<OperationResult<AdminsDto>> GetAdminsAsync(AdminsRequestDto dto)
        {
            return await GetAdminsQuery.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<AdminDto>> GetAdminByEmailAsync(string email)
        {
            return await GetAdminByEmailQuery.ExecuteAsync(email, _logger);
        }

        public async Task<OperationResult<AdminDto>> GetAdminByNameAsync(string fullName)
        {
            return await GetAdminByNameQuery.ExecuteAsync(fullName, _logger);
        }
    }
}
