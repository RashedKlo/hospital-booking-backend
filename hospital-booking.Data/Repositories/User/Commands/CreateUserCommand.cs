using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Settings;
using Microsoft.Extensions.Configuration;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data.Repositories.User.Helpers;
using hospital_booking.Data.Models;
using hospital_booking.Data.Results;
using System.Linq.Expressions;
using hospital_booking.Data.Helpers;

namespace hospital_booking.Data.Repositories.User.Commands
{
    public static class CreateUserCommand
    {
        private const string CreateUserSql = @"
INSERT INTO dbo.users (fullname, email, password, isGoogleLogin)
OUTPUT inserted.user_id, inserted.fullname, inserted.email, inserted.password
VALUES (@Fullname, @Email, @Password, 0);
";

        public static async Task<OperationResult<UserAuthenticationData>> ExecuteAsync(
            UserRegistrationDto dto,
            ILogger logger
           )
        {
            if (dto == null)
            {
                logger.LogError("CreateUserCommand received null registration data");
                return OperationResult<UserAuthenticationData>.Failure("Registration data is required");
            }

            logger.LogInformation("Executing user creation for Fullname: {Fullname}", dto.Fullname);

            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();

                using var command = CreateCommand(connection, dto);
                using var reader = await command.ExecuteReaderAsync();

                return await ProcessResultAsync(reader, logger, dto.Fullname);
            }

            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error during user creation for {Fullname}. Error: {Error}",
                    dto.Fullname, ex.Message);
                return OperationResult<UserAuthenticationData>.Failure("Database operation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during user creation for {Fullname}", dto.Fullname);
                return OperationResult<UserAuthenticationData>.Failure("User creation failed due to system error");
            }
        }

        private static SqlCommand CreateCommand(SqlConnection connection, UserRegistrationDto dto)
        {
            var command = new SqlCommand(CreateUserSql, connection);
            // Use parameters to prevent SQL injection and handle null values properly
            command.Parameters.AddWithValue("@Fullname", dto.Fullname);
            command.Parameters.AddWithValue("@Email", dto.Email);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            command.Parameters.AddWithValue("@Password", hashedPassword);
            return command;
        }


        private static async Task<OperationResult<UserAuthenticationData>> ProcessResultAsync(
            SqlDataReader reader,
            ILogger logger,
            string Fullname
           )
        {
            if (!await reader.ReadAsync())
            {
                logger.LogWarning("No result returned from registration procedure for {Fullname}", Fullname);
                return OperationResult<UserAuthenticationData>.Failure("Registration procedure returned no result");
            }
            else
            {
                var user = UserMapper.MapUserFromReader(reader);

                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var jwtSettings = JwtSettings.LoadFromConfiguration(config);
                var tokenHandler = new TokenHandler(jwtSettings);
                var accessToken = tokenHandler.GenerateAccessToken(user.UserId, user.Email, user.FullName);

                var authData = new UserAuthenticationData(user, accessToken);

                logger.LogInformation("User created successfully - UserId: {UserId}",
                    user.UserId);

                return OperationResult<UserAuthenticationData>.Success(authData, "User created successfully");
            }
        }

    }


}