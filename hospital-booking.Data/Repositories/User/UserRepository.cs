using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data.Helpers;
using hospital_booking.Data.Interfaces;
using hospital_booking.Data.Repositories.User.Commands;
using hospital_booking.Data.Repositories.User.Helpers;
using hospital_booking.Data.Repositories.User.Queries;
using hospital_booking.Data.Results;
using Microsoft.Extensions.Logging;
namespace hospital_booking.Data.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ILogger<UserRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<OperationResult<UserAuthenticationData>> CreateUserAsync(UserRegistrationDto user)
        {
            return await CreateUserCommand.ExecuteAsync(user, _logger);
        }
        public async Task<OperationResult<UserAuthenticationData>> LoginUserAsync(UserLoginDto user)
        {
            return await LoginUserCommand.ExecuteAsync(user, _logger);
        }


        public async Task<OperationResult<UserAuthenticationData>> AuthenticateGoogleAsync(string Email)
        {
            return await AuthenticateGoogleCommand.ExecuteAsync(Email, _logger);
        }

        public async Task<OperationResult<Models.User>> GetUserByEmailAsync(string Email)
        {
            return await GetUserByEmailQuery.ExecuteAsync(Email, _logger);
        }
    }
}