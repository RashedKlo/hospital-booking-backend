using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Prescription
{
    public class PrescriptionValidation
    {
        public static async Task<OperationResult<bool>> ValidatePrescriptionAsync(PrescriptionDto dto, IPrescriptionRepository prescriptionRepository, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Prescription data cannot be null");
                return OperationResult<bool>.Failure("Prescription data cannot be null");
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
