using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Interfaces
{
    public interface IAdminRepository
    {
        Task<OperationResult<AdminAuthenticationData>> RegisterAdminAsync(CreateAdminDto dto);
        Task<OperationResult<AdminAuthenticationData>> LoginAdminAsync(AdminLoginDto dto);
        Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, UpdateAdminDto dto);
        Task<OperationResult<bool>> DeleteAdminAsync(int adminId);
        Task<OperationResult<AdminDto>> GetAdminByIdAsync(int adminId);
        Task<OperationResult<List<AdminDto>>> GetAllAdminsAsync();
    }
}