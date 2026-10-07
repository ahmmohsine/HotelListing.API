namespace HotelListing.API.DTOs.ApplicationUser
{
    using System.ComponentModel.DataAnnotations;

    public record RegisterUserDto
    {
        [Required, EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; init; } = string.Empty;
        [Required, MaxLength(100)]
        public string FirstName { get; init; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; init; } = string.Empty;
        public string Role { get; set; } = "User";
    }

}
