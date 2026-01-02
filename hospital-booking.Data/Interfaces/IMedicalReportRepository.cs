using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IMedicalReportRepository
    {
        Task<OperationResult<MedicalReportDto>> GetMedicalReportAsync(int reportId);
        Task<OperationResult<MedicalReportsDto>> GetMedicalReportsAsync(MedicalReportsRequestDto requestDto);
        Task<OperationResult<bool>> CreateMedicalReportAsync(MedicalReportAddDto dto);
        Task<OperationResult<MedicalReportDto>> UpdateMedicalReportAsync(int reportId, MedicalReportUpdateDto dto);
        Task<OperationResult<bool>> DeleteMedicalReportAsync(int reportId);
    }
}
