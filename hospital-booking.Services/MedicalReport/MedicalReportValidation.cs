using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.MedicalReport
{
    public static class MedicalReportValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            MedicalReportAddDto dto, 
            IAppointmentRepository appointmentRepository, 
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Medical report add data cannot be null");
                return OperationResult<bool>.Failure("Medical report data is required");
            }

            if (dto.AppointmentId <= 0)
            {
                logger.LogError("Invalid appointment ID: {AppointmentId}", dto.AppointmentId);
                return OperationResult<bool>.Failure("Valid appointment ID is required");
            }

            // Check if appointment exists
            var appointmentResult = await appointmentRepository.GetAppointmentAsync(dto.AppointmentId);
            if (!appointmentResult.IsSuccess || appointmentResult.Data == null)
            {
                logger.LogWarning("Attempted to create medical report for non-existent appointment ID: {AppointmentId}", dto.AppointmentId);
                return OperationResult<bool>.Failure($"Appointment with ID {dto.AppointmentId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int reportId,
            MedicalReportUpdateDto dto, 
            IMedicalReportRepository medicalReportRepository,
            ILogger logger)
        {
            if (reportId <= 0)
            {
                logger.LogError("Invalid medical report ID: {ReportId}", reportId);
                return OperationResult<bool>.Failure("Invalid medical report ID");
            }

            if (dto == null)
            {
                logger.LogError("Medical report update data cannot be null");
                return OperationResult<bool>.Failure("Medical report update data is required");
            }

            // Check if medical report exists
            var existingResult = await medicalReportRepository.GetMedicalReportAsync(reportId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Medical report with ID {ReportId} not found for update", reportId);
                return OperationResult<bool>.Failure($"Medical report with ID {reportId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}

