using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<OperationResult<DoctorDto>> GetDoctorAsync(int doctorId);
        Task<OperationResult<List<DoctorDto>>> GetDoctorsAsync(int page, int limit);
        Task<OperationResult<DoctorDto>> CreateDoctorAsync(DoctorDto doctorDto);
        Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, DoctorDto doctorDto);
        Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId);
    }
}
