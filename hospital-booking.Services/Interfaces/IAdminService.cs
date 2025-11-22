using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IAdminService
    {
        Task<OperationResult<AdminDto>> GetAdminAsync(int adminId);
        Task<OperationResult<List<AdminDto>>> GetAdminsAsync(int page, int limit);
        Task<OperationResult<AdminDto>> CreateAdminAsync(AdminDto adminDto);
        Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminDto adminDto);
        Task<OperationResult<bool>> DeleteAdminAsync(int adminId);
    }
}
