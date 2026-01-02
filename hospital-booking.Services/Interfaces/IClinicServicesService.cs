using hospital_booking.Data.DTOs.ClinicService;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Services.Interfaces
{
    public interface IClinicServicesService
    {
        Task<OperationResult<ClinicServiceDto>> GetServiceAsync(int serviceId);
        Task<OperationResult<ClinicServicesDto>> GetServicesAsync(ClinicServicesRequestDto requestDto);
        Task<OperationResult<bool>> CreateServiceAsync(ClinicServiceAddDto dto);
        Task<OperationResult<ClinicServiceDto>> UpdateServiceAsync(int serviceId, ClinicServiceUpdateDto dto);
        Task<OperationResult<bool>> DeleteServiceAsync(int serviceId);
    }
}
