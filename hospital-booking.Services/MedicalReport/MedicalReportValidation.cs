using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.MedicalReport;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.MedicalReport
{
    public class MedicalReportValidation
    {
        public static async Task<OperationResult<bool>> ValidateMedicalReportAsync(MedicalReportDto dto, IMedicalReportRepository medicalReportRepository, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Medical report data cannot be null");
                return OperationResult<bool>.Failure("Medical report data cannot be null");
            }

            if (dto.AppointmentId <= 0)
            {
                logger.LogError("Valid appointment ID is required");
                return OperationResult<bool>.Failure("Valid appointment ID is required");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
