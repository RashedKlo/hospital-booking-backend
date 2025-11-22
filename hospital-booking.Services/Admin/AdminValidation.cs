using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Admin
{
    public class AdminValidation
    {
        public static async Task<OperationResult<bool>> ValidateAdminAsync(AdminDto dto, IAdminRepository adminRepository, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Admin data cannot be null");
                return OperationResult<bool>.Failure("Admin data cannot be null");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                logger.LogError("Admin email is required");
                return OperationResult<bool>.Failure("Email is required");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                logger.LogError("Admin full name is required");
                return OperationResult<bool>.Failure("Full name is required");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
