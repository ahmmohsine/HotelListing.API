using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.DTOs.ApplicationUser;
using HotelListing.API.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Services;

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
        var roleResult = await userManager.AddToRoleAsync(user, dto.Role);
        var registred = new RegisteredUserDto()
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Id = user.Id,
            Role = dto.Role
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

        return Result<string>.Success(await GenerateToken(user));
    }
    /* public async Task<Result<string>> LoginAsync(LoginUserDto dto)
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
*/
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

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        //Set basic user claims
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.FullName!)

            };
        //Set user role claims
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        claims = claims.Union(roleClaims).ToList();
        //Set Jwt key and credentials
        var securityKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //set Jwt token expiration and generate token
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Jwt:DurationInMinutes"])),
            signingCredentials: credentials
        );
        //return token
        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}
