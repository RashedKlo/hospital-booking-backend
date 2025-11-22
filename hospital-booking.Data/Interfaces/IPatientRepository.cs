using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IPatientRepository
    {
        Task<OperationResult<PatientDto>> GetPatientAsync(int patientId);
        Task<OperationResult<List<PatientDto>>> GetPatientsAsync(int page, int limit);
        Task<OperationResult<PatientDto>> CreatePatientAsync(PatientDto dto);
        Task<OperationResult<PatientDto>> UpdatePatientAsync(int patientId, PatientDto dto);
        Task<OperationResult<bool>> DeletePatientAsync(int patientId);
    }
}
