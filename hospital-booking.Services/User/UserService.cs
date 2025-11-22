using System;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Models;
using hospital_booking.Data.Repositories.User.Helpers;
using hospital_booking.Services.Interfaces;
using Microsoft.Extensions.Logging;
using hospital_booking.Data.Results;
using hospital_booking.Services.User;

namespace hospital_booking.Services.User
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<UserAuthenticationData>> RegisterUserAsync(UserRegistrationDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Registration attempted with null data");
                return OperationResult<UserAuthenticationData>.Failure("Registration data is required");
            }

            _logger.LogInformation("Processing user registration for Password: {Password}, email: {Email}",
                dto.Password, dto.Email);

            // Validate registration data
            var validationResult = await UserValidation.ValidateRegistrationAsync(dto, _userRepository, _logger);
            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Registration validation failed for {Fullname}: {Error}",
                    dto.Fullname, validationResult.Message);
                return OperationResult<UserAuthenticationData>.Failure(validationResult.Message);
            }

            // Create user via repository
            var createResult = await _userRepository.CreateUserAsync(dto);

            if (!createResult.IsSuccess)
            {
                _logger.LogWarning("User registration failed for {Fullname}: {Message}",
                    dto.Fullname, createResult.Message);
                return OperationResult<UserAuthenticationData>.Failure(createResult.Message);
            }

            _logger.LogInformation("User registered successfully - UserId: {UserId}",
                createResult.Data?.User?.UserId);

            return OperationResult<UserAuthenticationData>.Success(createResult.Data!, createResult.Message);
        }

        public async Task<OperationResult<UserAuthenticationData>> LoginUserAsync(UserLoginDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Login attempted with null data");
                return OperationResult<UserAuthenticationData>.Failure("Login credentials are required");
            }

            _logger.LogInformation("Processing user login for email: {Email}", dto.Email);

            var authResult = await _userRepository.LoginUserAsync(dto);

            if (!authResult.IsSuccess)
            {
                _logger.LogWarning("Login failed for email: {Email}: {Message}", dto.Email, authResult.Message);
                return OperationResult<UserAuthenticationData>.Failure(authResult.Message);
            }

            _logger.LogInformation("User logged in successfully - UserId: {UserId}",
                authResult.Data?.User?.UserId);

            return OperationResult<UserAuthenticationData>.Success(authResult.Data!, authResult.Message);
        }
        public async Task<OperationResult<UserAuthenticationData>> AuthenticateGoogleAsync(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                _logger.LogWarning("Login attempted with null data");
                return OperationResult<UserAuthenticationData>.Failure("Login credentials are required");
            }

            _logger.LogInformation("Processing user login for email: {Email}", Email);

            var authResult = await _userRepository.AuthenticateGoogleAsync(Email);

            if (!authResult.IsSuccess)
            {
                _logger.LogWarning("Login failed for email: {Email}: {Message}", Email, authResult.Message);
                return OperationResult<UserAuthenticationData>.Failure(authResult.Message);
            }

            _logger.LogInformation("User logged in successfully - UserId: {UserId}",
                authResult.Data?.User?.UserId);

            return OperationResult<UserAuthenticationData>.Success(authResult.Data!, authResult.Message);
        }




        public async Task<OperationResult<Data.Models.User>> GetUserByEmailAsync(string Email)
        {
            _logger.LogInformation("Fetching user by email: {Email}", Email);

            var result = await _userRepository.GetUserByEmailAsync(Email);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch user by email {Email}: {Message}", Email, result.Message);
                return OperationResult<Data.Models.User>.Failure(result.Message);
            }

            _logger.LogInformation("User fetched successfully - UserId: {UserId}, Email: {Email}",
                result.Data?.UserId, result.Data?.Email);

            return OperationResult<Data.Models.User>.Success(result.Data!, result.Message);
        }

    }


}