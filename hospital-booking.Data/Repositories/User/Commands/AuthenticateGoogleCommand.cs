using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data.Repositories.User.Helpers;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;
using hospital_booking.Data.Helpers;
using Microsoft.Extensions.Configuration;

namespace hospital_booking.Data.Repositories.User.Commands
{
    public class AuthenticateGoogleCommand
    {
        private const string AuthenticateGoogleSql = @"";

        public static async Task<OperationResult<UserAuthenticationData>> ExecuteAsync(
           string Email,
            ILogger logger)
        {

            logger.LogInformation("Executing Google authentication for email: {Email}", Email);

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
                logger.LogError(ex, "Database error during Google authentication for {Email}. Error: {Error}",
                    Email, ex.Message);
                return OperationResult<UserAuthenticationData>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during Google authentication for {Email}, Error: {Error}",
                    Email, ex.Message);
                return OperationResult<UserAuthenticationData>.Failure("Google authentication failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, string Email)
        {
            var command = new SqlCommand(AuthenticateGoogleSql, connection);

            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@FullName", Email.Substring(0, Email.IndexOf("@")));
            command.Parameters.AddWithValue("@isGoogleLogin", true);


            return command;
        }

        private static async Task<OperationResult<UserAuthenticationData>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
           string Email)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from Google authentication procedure for {Email}", Email);
                return OperationResult<UserAuthenticationData>.Failure("Google authentication procedure returned no result");
            }
            else
            {
                var user = UserMapper.MapUserFromReader(reader);

                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var jwtSettings = JwtSettings.LoadFromConfiguration(config);
                var tokenHandler = new TokenHandler(jwtSettings);
                var accessToken = tokenHandler.GenerateAccessToken(user.UserId, user.Email, user.FullName);

                var authData = new UserAuthenticationData(user, accessToken);

                logger.LogInformation("Google authentication successful - UserId: {UserId}",
                    user.UserId);

                return OperationResult<UserAuthenticationData>.Success(authData, "Google authentication successful");
            }
        }
    }


}