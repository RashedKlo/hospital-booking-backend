using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Appointment;
using hospital_booking.Data.DTOs.Admin; // For PaginationDto
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Appointment.Helpers;

namespace hospital_booking.Data.Repositories.Appointment.Queries
{
    public class GetAppointmentsQuery
    {
        public static async Task<OperationResult<AppointmentsDto>> ExecuteAsync(
            AppointmentsRequestDto requestDto, 
            ILogger logger)
        {
            // Validation
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetAppointmentsQuery received invalid params");
                return OperationResult<AppointmentsDto>.Failure("Invalid parameters");
            }

            try 
            {
                var (sql, parameters) = BuildQuery(requestDto);

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                using var reader = await command.ExecuteReaderAsync();
                
                // Read total count
                int totalCount = 0;
                if (await reader.ReadAsync())
                {
                    totalCount = reader.GetInt32(0);
                }

                await reader.NextResultAsync();

                var appointments = new List<AppointmentDto>();
                while (await reader.ReadAsync())
                {
                    appointments.Add(AppointmentMapper.MapFromReader(reader));
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var resultDto = new AppointmentsDto
                {
                    Appointments = appointments,
                    Pagination = new PaginationDto
                    {
                        Page = requestDto.Page,
                        PageSize = requestDto.Limit,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasPrevious = requestDto.Page > 1,
                        HasNext = requestDto.Page < totalPages
                    }
                };

                return OperationResult<AppointmentsDto>.Success(resultDto, "Appointments retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving appointments: {Error}", ex.Message);
                return OperationResult<AppointmentsDto>.Failure("Database operation failed");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(AppointmentsRequestDto request)
        {
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var search = $"%{request.SearchQuery.Trim()}%";
                whereConditions.Add("(p.full_name LIKE @Search OR d.full_name LIKE @Search OR a.reason LIKE @Search)");
                parameters.Add("@Search", search);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                whereConditions.Add("a.status = @Status");
                parameters.Add("@Status", request.Status);
            }

            if (request.DoctorId.HasValue)
            {
                whereConditions.Add("a.doctor_id = @DoctorId");
                parameters.Add("@DoctorId", request.DoctorId.Value);
            }

            if (request.PatientId.HasValue)
            {
                whereConditions.Add("a.patient_id = @PatientId");
                parameters.Add("@PatientId", request.PatientId.Value);
            }

            if (request.DateFrom.HasValue)
            {
                whereConditions.Add("a.appointment_time >= @DateFrom");
                parameters.Add("@DateFrom", request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                whereConditions.Add("a.appointment_time <= @DateTo");
                parameters.Add("@DateTo", request.DateTo.Value);
            }

            var whereClause = whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            var sql = $@"
-- Get Total Count
SELECT COUNT(*)
FROM dbo.appointments a
INNER JOIN dbo.patients p ON a.patient_id = p.patient_id
INNER JOIN dbo.doctors d ON a.doctor_id = d.doctor_id
{whereClause};

-- Get Data
SELECT 
    a.appointment_id, a.patient_id, a.doctor_id, a.appointment_time, a.reason, a.status,
    p.patient_id, p.full_name, p.birthDate, p.gender, p.notes,
    d.doctor_id, d.clinic_id, d.full_name, d.bio, d.phone, d.is_active, d.experience_years
FROM dbo.appointments a
INNER JOIN dbo.patients p ON a.patient_id = p.patient_id
INNER JOIN dbo.doctors d ON a.doctor_id = d.doctor_id
{whereClause}
ORDER BY a.appointment_time DESC
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return (sql, parameters);
        }
    }
}
