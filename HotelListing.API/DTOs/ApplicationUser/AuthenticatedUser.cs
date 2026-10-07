namespace HotelListing.API.DTOs.ApplicationUser
{
    public record AuthenticatedUser(string Id, string Email, IList<string> Roles);

}
