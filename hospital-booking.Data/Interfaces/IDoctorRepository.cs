using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Interfaces
{
    public interface IDoctorRepository
    {
        Task<OperationResult<DoctorAuthenticationData>> RegisterDoctorAsync(CreateDoctorDto dto);
        Task<OperationResult<DoctorAuthenticationData>> LoginDoctorAsync(DoctorLoginDto dto);
        Task<OperationResult<DoctorDto>> UpdateDoctorAsync(int doctorId, UpdateDoctorDto dto);
        Task<OperationResult<bool>> DeleteDoctorAsync(int doctorId);
        Task<OperationResult<DoctorDto>> GetDoctorByIdAsync(int doctorId);
        Task<OperationResult<List<DoctorDto>>> GetAllDoctorsAsync();
        Task<OperationResult<List<DoctorDto>>> GetDoctorsBySpecialtyAsync(int specialtyId);
    }
}