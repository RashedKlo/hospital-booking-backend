using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IDoctorRepository
    {
        Task<OperationResult<DoctorDto>> GetDoctorAsync(int doctorId);
        Task<OperationResult<DoctorsDto>> GetDoctorsAsync(DoctorsRequestDto requestDto);
        Task<OperationResult<bool>> CreateDoctorAsync(DoctorAddDto dto);
        Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, DoctorUpdateDto dto);
        Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId);
    }
}
