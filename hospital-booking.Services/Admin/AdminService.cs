using System;
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
            if (adminId <= 0)
            {
                return OperationResult<AdminDto>.Failure("Invalid admin ID");
            }
            
            _logger.LogInformation("Fetching admin by ID: {AdminId}", adminId);
            return await _adminRepository.GetAdminAsync(adminId);
        }

        public async Task<OperationResult<AdminDto>> GetAdminByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return OperationResult<AdminDto>.Failure("Email cannot be empty");
            }

            _logger.LogInformation("Fetching admin by email: {Email}", email);
            return await _adminRepository.GetAdminByEmailAsync(email);
        }

        public async Task<OperationResult<AdminDto>> GetAdminByNameAsync(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return OperationResult<AdminDto>.Failure("Name cannot be empty");
            }

            _logger.LogInformation("Fetching admin by name: {FullName}", fullName);
            return await _adminRepository.GetAdminByNameAsync(fullName);
        }   

        public async Task<OperationResult<AdminsDto>> GetAdminsAsync(AdminsRequestDto requestDto)
        {
            if (requestDto.Page < 1) requestDto.Page = 1;
            if (requestDto.Limit < 1) requestDto.Limit = 10;

            _logger.LogInformation("Fetching admins - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);
            return await _adminRepository.GetAdminsAsync(requestDto);
        }

        public async Task<OperationResult<AdminDto>> CreateAdminAsync(AdminAddDto adminDto)
        {
            _logger.LogInformation("Creating admin: {Email}", adminDto?.Email);

            var validation = await AdminValidation.ValidateAddAsync(adminDto!, _adminRepository, _logger);
            if (!validation.IsSuccess)
            {
                return OperationResult<AdminDto>.Failure(validation.Message);
            }

            return await _adminRepository.CreateAdminAsync(adminDto!);
        }

        public async Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminUpdateDto adminDto)
        {
            _logger.LogInformation("Updating admin: {AdminId}", adminId);

            var validation = await AdminValidation.ValidateUpdateAsync(adminId, adminDto, _adminRepository, _logger);
            if (!validation.IsSuccess)
            {
                return OperationResult<AdminDto>.Failure(validation.Message);
            }

            return await _adminRepository.UpdateAdminAsync(adminId, adminDto);
        }


        public async Task<OperationResult<bool>> DeleteAdminAsync(int adminId)
        {
            if (adminId <= 0)
            {
                return OperationResult<bool>.Failure("Invalid admin ID");
            }

            _logger.LogInformation("Deleting admin: {AdminId}", adminId);
            return await _adminRepository.DeleteAdminAsync(adminId);
        }
    }
}
