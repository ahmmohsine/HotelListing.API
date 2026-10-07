using HotelListing.API.Contracts;
using HotelListing.API.DTOs.ApplicationUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService _userService) : BaseApiController
    {

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var result = await _userService.RegisterAsync(registerUserDto);
            return ToActionResult(result);
            //Send Email
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
        {
            var result = await _userService.LoginAsync(loginUserDto);
            return ToActionResult(result);
        }
    }
}