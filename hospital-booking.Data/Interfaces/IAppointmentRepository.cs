using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.Results;
using System.Threading.Tasks;

namespace hospital_booking.Data.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<OperationResult<AppointmentDto>> GetAppointmentAsync(int appointmentId);
        Task<OperationResult<AppointmentsDto>> GetAppointmentsAsync(AppointmentsRequestDto requestDto);
        Task<OperationResult<bool>> CreateAppointmentAsync(AppointmentAddDto dto);
        Task<OperationResult<AppointmentDto>> UpdateAppointmentAsync(int appointmentId, AppointmentUpdateDto dto);
        Task<OperationResult<bool>> DeleteAppointmentAsync(int appointmentId);
    }
}
