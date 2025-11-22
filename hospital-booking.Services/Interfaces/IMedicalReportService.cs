using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IMedicalReportService
    {
        Task<OperationResult<MedicalReportDto>> GetMedicalReportAsync(int reportId);
        Task<OperationResult<List<MedicalReportDto>>> GetMedicalReportsAsync(int page, int limit);
        Task<OperationResult<List<MedicalReportDto>>> GetMedicalReportsByAppointmentAsync(int appointmentId);
        Task<OperationResult<MedicalReportDto>> CreateMedicalReportAsync(MedicalReportDto medicalReportDto);
        Task<OperationResult<MedicalReportDto>> UpdateMedicalReportAsync(int reportId, MedicalReportDto medicalReportDto);
        Task<OperationResult<bool>> DeleteMedicalReportAsync(int reportId);
    }
}
