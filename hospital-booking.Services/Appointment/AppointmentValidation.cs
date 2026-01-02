using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Services.Appointment
{
    public static class AppointmentValidation
    {
        public static async Task<OperationResult<bool>> ValidateAddAsync(
            AppointmentAddDto dto, 
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("Appointment add data cannot be null");
                return OperationResult<bool>.Failure("Appointment data is required");
            }

            if (dto.PatientId <= 0)
            {
                logger.LogError("Invalid patient ID: {PatientId}", dto.PatientId);
                return OperationResult<bool>.Failure("Valid patient ID is required");
            }

            if (dto.DoctorId <= 0)
            {
                logger.LogError("Invalid doctor ID: {DoctorId}", dto.DoctorId);
                return OperationResult<bool>.Failure("Valid doctor ID is required");
            }

            if (dto.AppointmentTime == default)
            {
                logger.LogError("Appointment time is required");
                return OperationResult<bool>.Failure("Appointment time is required");
            }

            // Check if patient exists
            var patientResult = await patientRepository.GetPatientAsync(dto.PatientId);
            if (!patientResult.IsSuccess || patientResult.Data == null)
            {
                logger.LogWarning("Attempted to create appointment for non-existent patient ID: {PatientId}", dto.PatientId);
                return OperationResult<bool>.Failure($"Patient with ID {dto.PatientId} does not exist");
            }

            // Check if doctor exists
            var doctorResult = await doctorRepository.GetDoctorAsync(dto.DoctorId);
            if (!doctorResult.IsSuccess || doctorResult.Data == null)
            {
                logger.LogWarning("Attempted to create appointment for non-existent doctor ID: {DoctorId}", dto.DoctorId);
                return OperationResult<bool>.Failure($"Doctor with ID {dto.DoctorId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }

        public static async Task<OperationResult<bool>> ValidateUpdateAsync(
            int appointmentId,
            AppointmentUpdateDto dto, 
            IAppointmentRepository appointmentRepository,
            ILogger logger)
        {
            if (appointmentId <= 0)
            {
                logger.LogError("Invalid appointment ID: {AppointmentId}", appointmentId);
                return OperationResult<bool>.Failure("Invalid appointment ID");
            }

            if (dto == null)
            {
                logger.LogError("Appointment update data cannot be null");
                return OperationResult<bool>.Failure("Appointment update data is required");
            }

            // Check if appointment exists
            var existingResult = await appointmentRepository.GetAppointmentAsync(appointmentId);
            if (!existingResult.IsSuccess || existingResult.Data == null)
            {
                logger.LogWarning("Appointment with ID {AppointmentId} not found for update", appointmentId);
                return OperationResult<bool>.Failure($"Appointment with ID {appointmentId} does not exist");
            }

            return OperationResult<bool>.Success(true);
        }
    }
}

