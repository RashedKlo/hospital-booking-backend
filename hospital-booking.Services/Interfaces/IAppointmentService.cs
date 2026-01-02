using System.Threading.Tasks;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<OperationResult<AppointmentDto>> GetAppointmentAsync(int appointmentId);
        Task<OperationResult<AppointmentsDto>> GetAppointmentsAsync(AppointmentsRequestDto requestDto);
        Task<OperationResult<bool>> CreateAppointmentAsync(AppointmentAddDto appointmentDto);
        Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentUpdateDto appointmentDto);
        Task<OperationResult<bool>> DeleteAppointmentAsync(int appointmentId);
    }
}
