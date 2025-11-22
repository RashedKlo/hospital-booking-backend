using System.Collections.Generic;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<OperationResult<AppointmentDto>> GetAppointmentAsync(int appointmentId);
        Task<OperationResult<List<AppointmentDto>>> GetAppointmentsAsync(int page, int limit);
        Task<OperationResult<AppointmentDto>> CreateAppointmentAsync(AppointmentDto appointmentDto);
        Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentDto appointmentDto);
        Task<OperationResult<bool>> DeleteAppointmentAsync(int appointmentId);
    }
}
