using HotelListing.API.Data;

namespace HotelListing.API.Contracts
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
