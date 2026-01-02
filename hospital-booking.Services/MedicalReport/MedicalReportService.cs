using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.MedicalReport
{
    public sealed class MedicalReportService : IMedicalReportService
    {
        private readonly IMedicalReportRepository _medicalReportRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<MedicalReportService> _logger;

        public MedicalReportService(
            IMedicalReportRepository medicalReportRepository, 
            IAppointmentRepository appointmentRepository,
            ILogger<MedicalReportService> logger)
        {
            _medicalReportRepository = medicalReportRepository ?? throw new ArgumentNullException(nameof(medicalReportRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<MedicalReportDto>> GetMedicalReportAsync(int reportId)
        {
            _logger.LogInformation("Fetching medical report by ID: {ReportId}", reportId);

            var result = await _medicalReportRepository.GetMedicalReportAsync(reportId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch medical report {ReportId}: {Message}", reportId, result.Message);
                return OperationResult<MedicalReportDto>.Failure(result.Message);
            }

            _logger.LogInformation("Medical report fetched successfully - ReportId: {ReportId}", result.Data?.ReportId);
            return OperationResult<MedicalReportDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<MedicalReportsDto>> GetMedicalReportsAsync(MedicalReportsRequestDto requestDto)
        {
            _logger.LogInformation("Fetching medical reports - Page: {Page}, Limit: {Limit}", requestDto.Page, requestDto.Limit);

            var result = await _medicalReportRepository.GetMedicalReportsAsync(requestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch medical reports: {Message}", result.Message);
                return OperationResult<MedicalReportsDto>.Failure(result.Message);
            }

            return OperationResult<MedicalReportsDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> CreateMedicalReportAsync(MedicalReportAddDto dto)
        {
            _logger.LogInformation("Creating medical report for AppointmentId: {AppointmentId}", dto?.AppointmentId);

            var validationResult = await MedicalReportValidation.ValidateAddAsync(dto!, _appointmentRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<bool>.Failure(validationResult.Message);
            }

            var result = await _medicalReportRepository.CreateMedicalReportAsync(dto!);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create medical report: {Message}", result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Medical report created successfully");
            return OperationResult<bool>.Success(true, result.Message);
        }

        public async Task<OperationResult<MedicalReportDto>> UpdateMedicalReportAsync(int reportId, MedicalReportUpdateDto dto)
        {
            _logger.LogInformation("Updating medical report: {ReportId}", reportId);

            var validationResult = await MedicalReportValidation.ValidateUpdateAsync(reportId, dto, _medicalReportRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<MedicalReportDto>.Failure(validationResult.Message);
            }

            var result = await _medicalReportRepository.UpdateMedicalReportAsync(reportId, dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to update medical report {ReportId}: {Message}", reportId, result.Message);
                return OperationResult<MedicalReportDto>.Failure(result.Message);
            }

            _logger.LogInformation("Medical report updated successfully - ReportId: {ReportId}", result.Data?.ReportId);
            return OperationResult<MedicalReportDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<bool>> DeleteMedicalReportAsync(int reportId)
        {
            _logger.LogInformation("Deleting medical report: {ReportId}", reportId);

            var result = await _medicalReportRepository.DeleteMedicalReportAsync(reportId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete medical report {ReportId}: {Message}", reportId, result.Message);
                return OperationResult<bool>.Failure(result.Message);
            }

            _logger.LogInformation("Medical report deleted successfully - ReportId: {ReportId}", reportId);
            return OperationResult<bool>.Success(result.Data, result.Message);
        }

    }
}
