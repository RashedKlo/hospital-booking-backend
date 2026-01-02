using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Prescription;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Prescription
{
    public static class PrescriptionValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            PrescriptionAddDto dto, 
            IAppointmentRepository appointmentRepository, 
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Prescription add data cannot be null");
                return OperationResult<bool>.Failure("Prescription data is required");
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
                logger.LogWarning("Attempted to create prescription for non-existent appointment ID: {AppointmentId}", dto.AppointmentId);
                return OperationResult<bool>.Failure($"Appointment with ID {dto.AppointmentId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int prescriptionId,
            PrescriptionUpdateDto dto, 
            IPrescriptionRepository prescriptionRepository,
            ILogger logger)

        {
            if (prescriptionId <= 0)
            {
                logger.LogError("Invalid prescription ID: {PrescriptionId}", prescriptionId);
                return OperationResult<bool>.Failure("Invalid prescription ID");
            }

            if (dto == null)
            {
                logger.LogError("Prescription update data cannot be null");
                return OperationResult<bool>.Failure("Prescription update data is required");
            }

            // Check if prescription exists
            var existingResult = await prescriptionRepository.GetPrescriptionAsync(prescriptionId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Prescription with ID {PrescriptionId} not found for update", prescriptionId);
                return OperationResult<bool>.Failure($"Prescription with ID {prescriptionId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}

