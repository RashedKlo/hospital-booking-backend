using System.Threading.Tasks;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IMedicalReportService
    {
        Task<OperationResult<MedicalReportDto>> GetMedicalReportAsync(int reportId);
        Task<OperationResult<MedicalReportsDto>> GetMedicalReportsAsync(MedicalReportsRequestDto requestDto);
        Task<OperationResult<bool>> CreateMedicalReportAsync(MedicalReportAddDto dto);
        Task<OperationResult<MedicalReportDto>> UpdateMedicalReportAsync(int reportId, MedicalReportUpdateDto dto);
        Task<OperationResult<bool>> DeleteMedicalReportAsync(int reportId);
    }
}
