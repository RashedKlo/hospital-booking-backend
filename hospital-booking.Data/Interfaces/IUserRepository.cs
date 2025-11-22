
using hospital_booking.Data.Results;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data.Repositories.User.Helpers;

namespace hospital_booking.Data.Interfaces
{
    public interface IUserRepository
    {

        Task<OperationResult<UserAuthenticationData>> CreateUserAsync(UserRegistrationDto dto);
        Task<OperationResult<UserAuthenticationData>> LoginUserAsync(UserLoginDto dto);
        Task<OperationResult<Models.User>> GetUserByEmailAsync(string Email);
        Task<OperationResult<UserAuthenticationData>> AuthenticateGoogleAsync(string Email);
    }
}