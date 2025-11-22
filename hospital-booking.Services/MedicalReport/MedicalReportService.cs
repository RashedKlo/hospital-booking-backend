using System;
using System.Collections.Generic;
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
        private readonly ILogger<MedicalReportService> _logger;

        public MedicalReportService(IMedicalReportRepository medicalReportRepository, ILogger<MedicalReportService> logger)
        {
            _medicalReportRepository = medicalReportRepository ?? throw new ArgumentNullException(nameof(medicalReportRepository));
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

        public async Task<OperationResult<List<MedicalReportDto>>> GetMedicalReportsAsync(int page, int limit)
        {
            _logger.LogInformation("Fetching medical reports - Page: {Page}, Limit: {Limit}", page, limit);

            var result = await _medicalReportRepository.GetMedicalReportsAsync(page, limit);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch medical reports: {Message}", result.Message);
                return OperationResult<List<MedicalReportDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} medical reports successfully", result.Data?.Count ?? 0);
            return OperationResult<List<MedicalReportDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<List<MedicalReportDto>>> GetMedicalReportsByAppointmentAsync(int appointmentId)
        {
            _logger.LogInformation("Fetching medical reports by AppointmentId: {AppointmentId}", appointmentId);

            var result = await _medicalReportRepository.GetMedicalReportsByAppointmentAsync(appointmentId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch medical reports for appointment {AppointmentId}: {Message}", appointmentId, result.Message);
                return OperationResult<List<MedicalReportDto>>.Failure(result.Message);
            }

            _logger.LogInformation("Fetched {Count} medical reports for AppointmentId: {AppointmentId}", result.Data?.Count ?? 0, appointmentId);
            return OperationResult<List<MedicalReportDto>>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<MedicalReportDto>> CreateMedicalReportAsync(MedicalReportDto medicalReportDto)
        {
            if (medicalReportDto == null)
            {
                _logger.LogWarning("Create medical report attempted with null data");
                return OperationResult<MedicalReportDto>.Failure("Medical report data is required");
            }

            _logger.LogInformation("Creating medical report for AppointmentId: {AppointmentId}", medicalReportDto.AppointmentId);

            var result = await _medicalReportRepository.CreateMedicalReportAsync(medicalReportDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to create medical report: {Message}", result.Message);
                return OperationResult<MedicalReportDto>.Failure(result.Message);
            }

            _logger.LogInformation("Medical report created successfully - ReportId: {ReportId}", result.Data?.ReportId);
            return OperationResult<MedicalReportDto>.Success(result.Data!, result.Message);
        }

        public async Task<OperationResult<MedicalReportDto>> UpdateMedicalReportAsync(int reportId, MedicalReportDto medicalReportDto)
        {
            if (medicalReportDto == null)
            {
                _logger.LogWarning("Update medical report attempted with null data");
                return OperationResult<MedicalReportDto>.Failure("Medical report data is required");
            }

            _logger.LogInformation("Updating medical report: {ReportId}", reportId);

            var result = await _medicalReportRepository.UpdateMedicalReportAsync(reportId, medicalReportDto);

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
