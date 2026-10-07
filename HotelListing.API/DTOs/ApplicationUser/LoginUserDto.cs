namespace HotelListing.API.DTOs.ApplicationUser
{
    using System.ComponentModel.DataAnnotations;

    public record LoginUserDto
    {
        [Required, EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; init; } = string.Empty;
    }


}
