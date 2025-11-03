using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.DTOs.Doctor;
using hospital_booking.Data.Repositories.Doctor.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Data.Repositories.Doctor.Queries
{
    public static class GetAllDoctorsQuery
    {
        private const string GetAllDoctorsSql = @"
            SELECT * FROM doctors 
            WHERE is_active = 1
            ORDER BY full_name";

        public static async Task<OperationResult<List<DoctorDto>>> ExecuteAsync(
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting all doctors");

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetAllDoctorsSql, connection);
                var doctors = new List<DoctorDto>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var doctor = DoctorMapper.MapDoctorFromReader(reader);
                    doctors.Add(DoctorMapper.MapToDto(doctor));
                }

                logger.LogDebug("Retrieved {Count} doctors", doctors.Count);
                return OperationResult<List<DoctorDto>>.Success(doctors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all doctors");
                return OperationResult<List<DoctorDto>>.Failure("Failed to retrieve doctors");
            }
        }
    }
}