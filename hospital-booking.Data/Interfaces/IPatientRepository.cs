using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Interfaces
{
    public interface IPatientRepository
    {
        Task<OperationResult<PatientAuthenticationData>> RegisterPatientAsync(PatientRegistrationDto dto);
        Task<OperationResult<PatientAuthenticationData>> LoginPatientAsync(PatientLoginDto dto);
        Task<OperationResult<PatientAuthenticationData>> GoogleLoginPatientAsync(PatientGoogleLoginDto dto);
        Task<OperationResult<PatientProfileDto>> GetPatientByIdAsync(int patientId);
        Task<OperationResult<PatientProfileDto>> GetPatientByEmailAsync(string email);
        Task<OperationResult<List<PatientProfileDto>>> GetAllPatientsAsync(int pageNumber = 1, int pageSize = 50);
        Task<OperationResult<PatientProfileDto>> UpdatePatientAsync(int patientId, PatientUpdateDto dto);
        Task<OperationResult<bool>> DeletePatientAsync(int patientId);
        Task<OperationResult<bool>> SuspendPatientAsync(int patientId);
        Task<OperationResult<bool>> ActivatePatientAsync(int patientId);
    }
}