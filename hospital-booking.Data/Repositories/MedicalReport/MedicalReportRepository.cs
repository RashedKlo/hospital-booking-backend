using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.MedicalReport.Commands;
using hospital_booking.Data.Repositories.MedicalReport.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.MedicalReport
{
    public class MedicalReportRepository : IMedicalReportRepository
    {
        private readonly ILogger<MedicalReportRepository> _logger;

        public MedicalReportRepository(ILogger<MedicalReportRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> CreateMedicalReportAsync(MedicalReportAddDto dto)
        {
            return await CreateMedicalReportCommand.ExecuteAsync(dto, _logger);
        }

        public async Task<OperationResult<MedicalReportDto>> UpdateMedicalReportAsync(int reportId, MedicalReportUpdateDto dto)
        {
            return await UpdateMedicalReportCommand.ExecuteAsync(reportId, dto, _logger);
        }

        public async Task<OperationResult<bool>> DeleteMedicalReportAsync(int reportId)
        {
            return await DeleteMedicalReportCommand.ExecuteAsync(reportId, _logger);
        }

        public async Task<OperationResult<MedicalReportDto>> GetMedicalReportAsync(int reportId)
        {
            return await GetMedicalReportQuery.ExecuteAsync(reportId, _logger);
        }

        public async Task<OperationResult<MedicalReportsDto>> GetMedicalReportsAsync(MedicalReportsRequestDto requestDto)
        {
            return await GetMedicalReportsQuery.ExecuteAsync(requestDto, _logger);
        }
    }
}
