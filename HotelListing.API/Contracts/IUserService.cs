using HotelListing.API.DTOs.ApplicationUser;
using HotelListing.API.Results;

namespace HotelListing.API.Contracts
{
    public interface IUserService
    {
        Task<Result<string>> LoginAsync(LoginUserDto dto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto dto);
        // Task ValidateCredentialsAsync(string email, string password);
        Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password);
    }
}