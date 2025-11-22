using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Patient;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IPatientService
    {
        Task<OperationResult<PatientDto>> GetPatientAsync(int patientId);
        Task<OperationResult<List<PatientDto>>> GetPatientsAsync(int page, int limit);
        Task<OperationResult<PatientDto>> CreatePatientAsync(PatientDto patientDto);
        Task<OperationResult<PatientDto>> UpdatePatientAsync(int patientId, PatientDto patientDto);
        Task<OperationResult<bool>> DeletePatientAsync(int patientId);
    }
}
