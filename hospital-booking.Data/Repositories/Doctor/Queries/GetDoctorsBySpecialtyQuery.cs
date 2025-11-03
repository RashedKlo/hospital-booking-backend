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
    public static class GetDoctorsBySpecialtyQuery
    {
        private const string GetDoctorsSql = @"
            SELECT * FROM doctors 
            WHERE specialty_id = @SpecialtyId AND is_active = 1
            ORDER BY rating DESC, full_name";

        public static async Task<OperationResult<List<DoctorDto>>> ExecuteAsync(
            int specialtyId,
            ILogger logger,
            string connectionString)
        {
            logger.LogDebug("Getting doctors for specialty: {SpecialtyId}", specialtyId);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(GetDoctorsSql, connection);
                command.Parameters.AddWithValue("@SpecialtyId", specialtyId);

                var doctors = new List<DoctorDto>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var doctor = DoctorMapper.MapDoctorFromReader(reader);
                    doctors.Add(DoctorMapper.MapToDto(doctor));
                }

                logger.LogDebug("Retrieved {Count} doctors for specialty {SpecialtyId}", doctors.Count, specialtyId);
                return OperationResult<List<DoctorDto>>.Success(doctors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting doctors for specialty: {SpecialtyId}", specialtyId);
                return OperationResult<List<DoctorDto>>.Failure("Failed to retrieve doctors");
            }
        }
    }
}