using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Repositories.User.Helpers;
using hospital_booking.Data.Results;
using hospital_booking.Data.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace hospital_booking.Data.Repositories.User.Queries
{
    public class GetUserByEmailQuery
    {

        private const string GetUserSql = @"
    SELECT TOP (1)
        user_id,
        fullname,
        email,
        password
            FROM dbo.users
    WHERE email = @Email;
    ";

        public static async Task<OperationResult<Models.User>> ExecuteAsync(
           string Email,
            ILogger logger)
        {
            if (string.IsNullOrEmpty(Email))
            {
                logger.LogError("GetUserByEmailQuery received Empty Email data");
                return OperationResult<Models.User>.Failure(" Email is required");
            }

            logger.LogInformation("Executing getting user by Email: {Email}", Email);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, Email);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, Email);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during getting user by {Email}. Error: {Error}",
                    Email, ex.Message);
                return OperationResult<Models.User>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during getting user by {Email}", Email);
                return OperationResult<Models.User>.Failure("Getting user failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, string Email)
        {
            var command = new SqlCommand(GetUserSql, connection);
            command.Parameters.AddWithValue("@Email", Email);
            return command;
        }



        private static async Task<OperationResult<Models.User>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            string Email)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from getting user by email procedure for {Email}", Email);
                return OperationResult<Models.User>.Failure("User is not found");
            }
            else
            {
                var user = UserMapper.MapUserFromReader(reader);
                logger.LogInformation("Getting user by email successfully - UserId: {UserId}, Email: {Email}",
                    user.UserId, user.Email);

                return OperationResult<Models.User>.Success(user, "User is found successfully");
            }
        }
    }
}
