using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IDoctorRepository
    {
        Task<OperationResult<DoctorDto>> GetDoctorAsync(int doctorId);
        Task<OperationResult<List<DoctorDto>>> GetDoctorsAsync(int page, int limit);
        Task<OperationResult<DoctorDto>> CreateDoctorAsync(DoctorDto dto);
        Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, DoctorDto dto);
        Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId);
    }
}
