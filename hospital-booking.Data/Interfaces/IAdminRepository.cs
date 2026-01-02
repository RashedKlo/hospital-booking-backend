using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IAdminRepository
    {
        Task<OperationResult<AdminDto>> GetAdminAsync(int adminId);
        Task<OperationResult<AdminsDto>> GetAdminsAsync(AdminsRequestDto dto);
        Task<OperationResult<AdminDto>> CreateAdminAsync(AdminAddDto dto);
        Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminUpdateDto dto);
        Task<OperationResult<bool>> DeleteAdminAsync(int adminId);
        Task<OperationResult<AdminDto>> GetAdminByEmailAsync(string email); 
        Task<OperationResult<AdminDto>> GetAdminByNameAsync(string fullName);
    }   
}
