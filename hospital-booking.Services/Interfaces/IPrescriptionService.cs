using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<OperationResult<PrescriptionDto>> GetPrescriptionAsync(int prescriptionId);
        Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsAsync(int page, int limit);
        Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsByAppointmentAsync(int appointmentId);
        Task<OperationResult<PrescriptionDto>> CreatePrescriptionAsync(PrescriptionDto prescriptionDto);
        Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionDto prescriptionDto);
        Task<OperationResult<bool>> DeletePrescriptionAsync(int prescriptionId);
    }
}
