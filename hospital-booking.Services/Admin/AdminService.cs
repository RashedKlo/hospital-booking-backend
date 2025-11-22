using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Admin
{
    public sealed class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository ?? throw new ArgumentNullException(nameof(adminRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<AdminDto>> GetAdminAsync(int adminId)
        {
            _logger.LogInformation("Fetching admin by ID: {AdminId}", adminId);

            var result = await _adminRepository.GetAdminAsync(adminId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch admin {AdminId}: {Message}", adminId, result.Message);
                return OperationResult<AdminDto>.Failure(result.Message);
            }

            _logger.LogInformation("Admin fetched successfully - AdminId: {AdminId}", result.Data?.AdminId);
            return OperationResult<AdminDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<List<AdminDto>>> GetAdminsAsync(int page, int limit)
        {
            _logger.LogInformation("Fetching admins - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _adminRepository.GetAdminsAsync(page, limit);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch admins: {Message}", result.Message);
                return OperationResult<List<AdminDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} admins successfully", result.Data?.Count ?? 0);
            return OperationResult<List<AdminDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<AdminDto>> CreateAdminAsync(AdminDto adminDto)
        {
            if (adminDto == null)
            {
                _logger.LogWarning("Create admin attempted with null data");
                return OperationResult<AdminDto>.Failure("Admin data is required");
            }

            _logger.LogInformation("Creating admin: {Email}", adminDto.Email);

            var result = await _adminRepository.CreateAdminAsync(adminDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create admin: {Message}", result.Message);
                return OperationResult<AdminDto>.Failure(result.Message);
            }

            _logger.LogInformation("Admin created successfully - AdminId: {AdminId}", result.Data?.AdminId);
            return OperationResult<AdminDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminDto adminDto)
        {
            if (adminDto == null)
            {
                _logger.LogWarning("Update admin attempted with null data");
                return OperationResult<AdminDto>.Failure("Admin data is required");
            }

            _logger.LogInformation("Updating admin: {AdminId}", adminId);

            var result = await _adminRepository.UpdateAdminAsync(adminId, adminDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update admin {AdminId}: {Message}", adminId, result.Message);
                return OperationResult<AdminDto>.Failure(result.Message);
            }

            _logger.LogInformation("Admin updated successfully - AdminId: {AdminId}", result.Data?.AdminId);
            return OperationResult<AdminDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> DeleteAdminAsync(int adminId)
        {
            _logger.LogInformation("Deleting admin: {AdminId}", adminId);

            var result = await _adminRepository.DeleteAdminAsync(adminId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete admin {AdminId}: {Message}", adminId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Admin deleted successfully - AdminId: {AdminId}", adminId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }
    }
}
