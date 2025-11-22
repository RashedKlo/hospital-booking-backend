using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Interfaces
{
    public interface IMedicalReportRepository
    {
        /// <summary>
        /// Get a single medical report by report ID
        /// </summary>
        Task<OperationResult<MedicalReportDto>> GetMedicalReportAsync(int reportId);

        /// <summary>
        /// Get all medical reports with pagination
        /// </summary>
        Task<OperationResult<List<MedicalReportDto>>> GetMedicalReportsAsync(int page, int limit);

        /// <summary>
        /// Get medical reports by appointment ID
        /// </summary>
        Task<OperationResult<List<MedicalReportDto>>> GetMedicalReportsByAppointmentAsync(int appointmentId);

        /// <summary>
        /// Create a new medical report
        /// </summary>
        Task<OperationResult<MedicalReportDto>> CreateMedicalReportAsync(MedicalReportDto medicalReportDto);

        /// <summary>
        /// Update an existing medical report
        /// </summary>
        Task<OperationResult<MedicalReportDto>> UpdateMedicalReportAsync(int reportId, MedicalReportDto medicalReportDto);

        /// <summary>
        /// Delete a medical report by report ID
        /// </summary>
        Task<OperationResult<bool>> DeleteMedicalReportAsync(int reportId);
    }
}
