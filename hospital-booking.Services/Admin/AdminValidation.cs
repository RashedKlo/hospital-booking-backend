using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Admin
{
    public static class AdminValidation
    {
        // Pre-compile regex for better performance
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PhoneRegex = new Regex(
            @"^\+?[1-9]\d{1,14}$",
            RegexOptions.Compiled);

        // Define valid roles
        private static readonly HashSet<string> ValidRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SuperAdmin", "Admin", "Manager", "Support"
        };

        public static async Task<OperationResult<bool>> ValidateAddAsync(
            AdminAddDto dto, 
            IAdminRepository adminRepository, 
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("AdminAddDto is null");
                return OperationResult<bool>.Failure("Invalid data");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName)) return OperationResult<bool>.Failure("Full name is required");
            if (dto.FullName.Length > 250) return OperationResult<bool>.Failure("Full name cannot exceed 250 characters");

            if (string.IsNullOrWhiteSpace(dto.Email)) return OperationResult<bool>.Failure("Email is required");
            if (!EmailRegex.IsMatch(dto.Email)) return OperationResult<bool>.Failure("Invalid email format");

            if (string.IsNullOrWhiteSpace(dto.Role)) return OperationResult<bool>.Failure("Role is required");
            if (!ValidRoles.Contains(dto.Role)) return OperationResult<bool>.Failure($"Invalid role. Valid roles: {string.Join(", ", ValidRoles)}");

            if (!string.IsNullOrEmpty(dto.Phone) && !PhoneRegex.IsMatch(dto.Phone)) return OperationResult<bool>.Failure("Invalid phone number");

            // Email uniqueness check
            var existingAdmin = await adminRepository.GetAdminByEmailAsync(dto.Email);
            if (existingAdmin.IsSuccess && existingAdmin.Data != null)
            {
                logger.LogWarning("Admin with email {Email} already exists", dto.Email);
                return OperationResult<bool>.Failure("Email is already in use");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int adminId,
            AdminUpdateDto dto, 
            IAdminRepository adminRepository, 
            ILogger logger)
        {
            if (adminId <= 0) return OperationResult<bool>.Failure("Invalid admin ID");
            if (dto == null) return OperationResult<bool>.Failure("Invalid data");

            // Check if admin exists
            var existingRecord = await adminRepository.GetAdminAsync(adminId);
            if (!existingRecord.IsSuccess || existingRecord.Data == null)
            {
                logger.LogWarning("Admin with ID {AdminId} not found for update", adminId);
                return OperationResult<bool>.Failure($"Admin with ID {adminId} does not exist");
            }

            if (dto.FullName != null && string.IsNullOrWhiteSpace(dto.FullName)) return OperationResult<bool>.Failure("Full name cannot be empty");
            
            if (dto.Email != null) 
            {
                if (string.IsNullOrWhiteSpace(dto.Email)) return OperationResult<bool>.Failure("Email cannot be empty");
                if (!EmailRegex.IsMatch(dto.Email)) return OperationResult<bool>.Failure("Invalid email format");

                // Email uniqueness if email is changed
                var existingByEmail = await adminRepository.GetAdminByEmailAsync(dto.Email);
                if (existingByEmail.IsSuccess && existingByEmail.Data != null && existingByEmail.Data.AdminId != adminId)
                {
                    logger.LogWarning("Email {Email} is already used by another admin", dto.Email);
                    return OperationResult<bool>.Failure("Email is already in use by another admin");
                }
            }
            
            if (dto.Role != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Role)) return OperationResult<bool>.Failure("Role cannot be empty");
                if (!ValidRoles.Contains(dto.Role)) return OperationResult<bool>.Failure($"Invalid role. Valid roles: {string.Join(", ", ValidRoles)}");
            }

            if (dto.Phone != null && !PhoneRegex.IsMatch(dto.Phone)) return OperationResult<bool>.Failure("Invalid phone number");

            return OperationResult<bool>.Success(true);
        }
    }
}
