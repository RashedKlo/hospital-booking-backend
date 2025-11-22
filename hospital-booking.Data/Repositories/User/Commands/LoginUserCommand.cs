using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hospital_booking.Data.Results;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data.Repositories.User.Helpers;
using hospital_booking.Data.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Helpers;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace hospital_booking.Data.Repositories.User.Commands
{
    public class LoginUserCommand
    {
        private const string GetUserSql = @"
SELECT 
    user_id,
    fullname,
    email,
    password,
    isGoogleLogin
FROM dbo.users
WHERE email = @Email;
";

        public static async Task<OperationResult<UserAuthenticationData>> ExecuteAsync(
            UserLoginDto dto,
            ILogger logger)
        {
            if (dto == null)
            {
                logger.LogError("LoginUserCommand received null login data");
                return OperationResult<UserAuthenticationData>.Failure("Login data is required");
            }

            logger.LogInformation("Executing user Login for Email: {Email}", dto.Email);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, dto.Email, dto.Password);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during user login for {Email}. Error: {Error}",
                    dto.Email, ex.Message);
                return OperationResult<UserAuthenticationData>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during user login for {Email}", dto.Email);
                return OperationResult<UserAuthenticationData>.Failure("User login failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, UserLoginDto dto)
        {
            var command = new SqlCommand(GetUserSql, connection);
            command.Parameters.AddWithValue("@Email", dto.Email);
            return command;
        }

        private static async Task<OperationResult<UserAuthenticationData>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            string Email,
            string Password)
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No user found with email: {Email}", Email);
                return OperationResult<UserAuthenticationData>.Failure("Invalid email or password");
            }

            var user = UserMapper.MapUserFromReader(reader);
            if (Password != user.Password)
            {
                logger.LogWarning("Invalid password attempt for email: {Email}", Email);
                return OperationResult<UserAuthenticationData>.Failure("Invalid email or password");
            }
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var jwtSettings = JwtSettings.LoadFromConfiguration(config);
            var tokenHandler = new TokenHandler(jwtSettings);
            var accessToken = tokenHandler.GenerateAccessToken(user.UserId, user.Email, user.FullName);

            var authData = new UserAuthenticationData(user, accessToken);

            logger.LogInformation("User login successful - UserId: {UserId}",
                user.UserId);

            return OperationResult<UserAuthenticationData>.Success(authData, "User login successful");
        }
    }
}