using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data;
using hospital_booking.Data.Repositories.User.Helpers;
using hospital_booking.Data.Results;

namespace hospital_booking.Services.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult<UserAuthenticationData>> RegisterUserAsync(UserRegistrationDto dto);
        Task<OperationResult<UserAuthenticationData>> LoginUserAsync(UserLoginDto dto);
        Task<OperationResult<UserAuthenticationData>> AuthenticateGoogleAsync(string Email);
        Task<OperationResult<Data.Models.User>> GetUserByEmailAsync(string Email);
    }
}