using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IPatientRepository
    {
        Task<OperationResult<PatientDto>> GetPatientAsync(int patientId);
        Task<OperationResult<PatientsDto>> GetPatientsAsync(PatientsRequestDto requestDto);
        Task<OperationResult<bool>> CreatePatientAsync(PatientAddDto dto);
        Task<OperationResult<PatientDto>> UpdatePatientAsync(int patientId, PatientUpdateDto dto);
        Task<OperationResult<bool>> DeletePatientAsync(int patientId);
    }
}
