using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IAdminRepository
    {
        Task<OperationResult<AdminDto>> GetAdminAsync(int adminId);
        Task<OperationResult<List<AdminDto>>> GetAdminsAsync(int page, int limit);
        Task<OperationResult<AdminDto>> CreateAdminAsync(AdminDto dto);
        Task<OperationResult<AdminDto>> UpdateAdminAsync(int adminId, AdminDto dto);
        Task<OperationResult<bool>> DeleteAdminAsync(int adminId);
    }
}
