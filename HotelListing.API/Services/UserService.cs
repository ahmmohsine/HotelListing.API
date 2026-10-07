using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.DTOs.ApplicationUser;
using HotelListing.API.Results;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IUserService
    {
        public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto dto)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
                return Result<RegisteredUserDto>.BadRequest(errors);
            }

            var registred = new RegisteredUserDto()
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Id = user.Id
            };
            return Result<RegisteredUserDto>.Success(registred);
        }
        public async Task<Result<string>> LoginAsync(LoginUserDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user is null)
            {
                return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));
            }

            var is_valid = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!is_valid)
            {
                return Result<string>.Failure(new Error("BadRequest", "The provided credentials are invalid."));
            }

            return Result<string>.Success("Login successful.");
        }

        public async Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null || !await userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);
            return new AuthenticatedUser(user.Id, user.Email!, roles);
        }
    }
}
