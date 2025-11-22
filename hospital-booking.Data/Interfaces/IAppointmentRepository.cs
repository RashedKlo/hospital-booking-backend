using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<OperationResult<AppointmentDto>> GetAppointmentAsync(int appointmentId);
        Task<OperationResult<List<AppointmentDto>>> GetAppointmentsAsync(int page, int limit);
        Task<OperationResult<AppointmentDto>> CreateAppointmentAsync(AppointmentDto dto);
        Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentDto dto);
        Task<OperationResult<bool>> DeleteAppointmentAsync(int appointmentId);
    }
}
