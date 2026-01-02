using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<OperationResult<DoctorDto>> GetDoctorAsync(int doctorId);
        Task<OperationResult<DoctorsDto>> GetDoctorsAsync(DoctorsRequestDto requestDto);
        Task<OperationResult<bool>> CreateDoctorAsync(DoctorAddDto doctorDto);
        Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, DoctorUpdateDto doctorDto);
        Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId);
    }
}
