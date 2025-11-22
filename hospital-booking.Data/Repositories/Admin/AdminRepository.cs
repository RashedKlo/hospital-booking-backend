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

        public async Task<OperationResult<AdminDto>> CreateAdminAsync(AdminDto dto)
        {
            return await CreateAdminCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminDto dto)
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

        public async Task<OperationResult<List<AdminDto>>> GetAdminsAsync(int page, int limit)
        {
            return await GetAdminsQuery.ExecuteAsync(page, limit, _logger);
        }
    }
}
