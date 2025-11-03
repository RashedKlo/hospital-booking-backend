using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Models;
using hospital_booking.Data.Repositories.Admin.Commands;
using hospital_booking.Data.Repositories.Admin.Queries;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace hospital_booking.Data.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ILogger<AdminRepository> _logger;
        private readonly TokenHandler _tokenHandler;
        private readonly string _connectionString;

        public AdminRepository(
            ILogger<AdminRepository> logger,
            TokenHandler tokenHandler,
            IOptions<DatabaseSettings> databaseSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenHandler = tokenHandler ?? throw new ArgumentNullException(nameof(tokenHandler));
            _connectionString = databaseSettings?.Value?.ConnectionString 
                ?? throw new ArgumentNullException(nameof(databaseSettings));
        }

        public async Task<OperationResult<AdminAuthenticationData>> RegisterAdminAsync(CreateAdminDto dto)
        {
            return await RegisterAdminCommand.ExecuteAsync(dto, _logger, _tokenHandler, _connectionString);
        }

        public async Task<OperationResult<AdminAuthenticationData>> LoginAdminAsync(AdminLoginDto dto)
        {
            return await LoginAdminCommand.ExecuteAsync(dto, _logger, _tokenHandler, _connectionString);
        }

        public async Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, UpdateAdminDto dto)
        {
            return await UpdateAdminCommand.ExecuteAsync(adminId, dto, _logger, _connectionString);
        }

        public async Task<OperationResult<bool>> DeleteAdminAsync(int adminId)
        {
            return await DeleteAdminCommand.ExecuteAsync(adminId, _logger, _connectionString);
        }

        public async Task<OperationResult<AdminDto>> GetAdminByIdAsync(int adminId)
        {
            return await GetAdminByIdQuery.ExecuteAsync(adminId, _logger, _connectionString);
        }

        public async Task<OperationResult<List<AdminDto>>> GetAllAdminsAsync()
        {
            return await GetAllAdminsQuery.ExecuteAsync(_logger, _connectionString);
        }
    }
}