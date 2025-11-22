using hospital_booking.Data.DTOs.Clinic;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Interfaces
{
    public interface IClinicRepository
    {
        /// <summary>
        /// Get a single clinic by clinic ID
        /// </summary>
        Task<OperationResult<ClinicDto>> GetClinicAsync(int clinicId);

        /// <summary>
        /// Get all clinics with pagination
        /// </summary>
        Task<OperationResult<List<ClinicDto>>> GetClinicsAsync(int page, int limit);

        /// <summary>
        /// Create a new clinic
        /// </summary>
        Task<OperationResult<ClinicDto>> CreateClinicAsync(ClinicDto clinicDto);

        /// <summary>
        /// Update an existing clinic
        /// </summary>
        Task<OperationResult<ClinicDto>> UpdateClinicAsync(int clinicId, ClinicDto clinicDto);

        /// <summary>
        /// Delete a clinic by clinic ID
        /// </summary>
        Task<OperationResult<bool>> DeleteClinicAsync(int clinicId);
    }
}
