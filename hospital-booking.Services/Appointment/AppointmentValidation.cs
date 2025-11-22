using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Appointment
{
    public class AppointmentValidation
    {
        public static async Task<OperationResult<bool>> ValidateAppointmentAsync(AppointmentDto dto, IAppointmentRepository appointmentRepository, ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Appointment data cannot be null");
                return OperationResult<bool>.Failure("Appointment data cannot be null");
            }

            if (dto.PatientId <= 0)
            {
                logger.LogError("Valid patient ID is required");
                return OperationResult<bool>.Failure("Valid patient ID is required");
            }

            if (dto.DoctorId <= 0)
            {
                logger.LogError("Valid doctor ID is required");
                return OperationResult<bool>.Failure("Valid doctor ID is required");
            }

            if (dto.AppointmentTime == default)
            {
                logger.LogError("Appointment time is required");
                return OperationResult<bool>.Failure("Appointment time is required");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
