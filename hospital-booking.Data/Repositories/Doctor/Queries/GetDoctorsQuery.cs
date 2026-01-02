using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.DTOs.Admin;
using hospital_booking.Data.Results;
using hospital_booking.Data.Repositories.Doctor.Helpers;

namespace hospital_booking.Data.Repositories.Doctor.Queries
{
    public class GetDoctorsQuery
    {
        public static async Task<OperationResult<DoctorsDto>> ExecuteAsync(
            DoctorsRequestDto requestDto, 
            ILogger logger)
        {
            if (requestDto == null || requestDto.Page <= 0 || requestDto.Limit <= 0)
            {
                logger.LogError("GetDoctorsQuery received invalid params");
                return OperationResult<DoctorsDto>.Failure("Invalid parameters");
            }

            try 
            {
                var (sql, parameters) = BuildQuery(requestDto);

                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(sql, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }

                using var reader = await command.ExecuteReaderAsync();
                
                int totalCount = 0;
                if (await reader.ReadAsync())
                {
                    totalCount = reader.GetInt32(0);
                }

                await reader.NextResultAsync();

                var doctors = new List<DoctorDto>();
                while (await reader.ReadAsync())
                {
                    doctors.Add(DoctorMapper.MapFromReader(reader));
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.Limit);
                var resultDto = new DoctorsDto
                {
                    Doctors = doctors,
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

                return OperationResult<DoctorsDto>.Success(resultDto, "Doctors retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving doctors: {Error}", ex.Message);
                return OperationResult<DoctorsDto>.Failure("Database operation failed");
            }
        }

        private static (string sql, Dictionary<string, object> parameters) BuildQuery(DoctorsRequestDto request)
        {
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            var offset = (request.Page - 1) * request.Limit;
            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", request.Limit);

            // Filters
            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var search = $"%{request.SearchQuery.Trim()}%";
                // Searching doctor name or bio, or clinic name?
                // Let's search doctor name, bio, and clinic name
                whereConditions.Add("(d.full_name LIKE @Search OR d.bio LIKE @Search OR c.name LIKE @Search)");
                parameters.Add("@Search", search);
            }

            if (request.ClinicId.HasValue)
            {
                whereConditions.Add("d.clinic_id = @ClinicId");
                parameters.Add("@ClinicId", request.ClinicId.Value);
            }

            if (request.MinExperienceYears.HasValue)
            {
                whereConditions.Add("d.experience_years >= @MinExperience");
                parameters.Add("@MinExperience", request.MinExperienceYears.Value);
            }

            var whereClause = whereConditions.Count > 0 
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : "";

            var sql = $@"
-- Count
SELECT COUNT(*) 
FROM dbo.doctors d
INNER JOIN dbo.clinics c ON d.clinic_id = c.clinic_id
{whereClause};

-- Data
SELECT 
    d.doctor_id, d.clinic_id, d.full_name, d.bio, d.phone, d.is_active, d.experience_years,
    c.clinic_id, c.name, c.description, c.address, c.phone, c.email, c.website, c.image_url, 
    c.rating, c.review_count, c.opening_hours, c.latitude, c.longitude, c.created_at, c.updated_at
FROM dbo.doctors d
INNER JOIN dbo.clinics c ON d.clinic_id = c.clinic_id
{whereClause}
ORDER BY d.doctor_id
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return (sql, parameters);
        }
    }
}
