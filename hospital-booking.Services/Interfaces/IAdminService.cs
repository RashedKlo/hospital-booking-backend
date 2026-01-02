using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IAdminService
    {
        Task<OperationResult<AdminDto>> GetAdminAsync(int adminId);
        Task<OperationResult<AdminsDto>> GetAdminsAsync(AdminsRequestDto requestDto);
        Task<OperationResult<AdminDto>> CreateAdminAsync(AdminAddDto adminDto);
        Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminUpdateDto adminDto);
        Task<OperationResult<bool>> DeleteAdminAsync(int adminId);
        Task<OperationResult<AdminDto>> GetAdminByEmailAsync(string email);
        Task<OperationResult<AdminDto>> GetAdminByNameAsync(string fullName);
    }
}
