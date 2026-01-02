using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IPatientService
    {
        Task<OperationResult<PatientDto>> GetPatientAsync(int patientId);
        Task<OperationResult<PatientsDto>> GetPatientsAsync(PatientsRequestDto requestDto);
        Task<OperationResult<bool>> CreatePatientAsync(PatientAddDto dto);
        Task<OperationResult<PatientDto>> UpdatePatientAsync(int patientId, PatientUpdateDto dto);
        Task<OperationResult<bool>> DeletePatientAsync(int patientId);
    }
}
