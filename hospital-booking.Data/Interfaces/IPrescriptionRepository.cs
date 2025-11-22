using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Interfaces
{
    public interface IPrescriptionRepository
    {
        /// <summary>
        /// Get a single prescription by prescription ID
        /// </summary>
        Task<OperationResult<PrescriptionDto>> GetPrescriptionAsync(int prescriptionId);

        /// <summary>
        /// Get all prescriptions with pagination
        /// </summary>
        Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsAsync(int page, int limit);

        /// <summary>
        /// Get prescriptions by appointment ID
        /// </summary>
        Task<OperationResult<List<PrescriptionDto>>> GetPrescriptionsByAppointmentAsync(int appointmentId);

        /// <summary>
        /// Create a new prescription
        /// </summary>
        Task<OperationResult<PrescriptionDto>> CreatePrescriptionAsync(PrescriptionDto prescriptionDto);

        /// <summary>
        /// Update an existing prescription
        /// </summary>
        Task<OperationResult<PrescriptionDto>> UpdatePrescriptionAsync(int prescriptionId, PrescriptionDto prescriptionDto);

        /// <summary>
        /// Delete a prescription by prescription ID
        /// </summary>
        Task<OperationResult<bool>> DeletePrescriptionAsync(int prescriptionId);
    }
}
