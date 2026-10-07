using HotelListing.API.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly HotelListingDbContext _context;

        public ApiKeyRepository(HotelListingDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken token = default)
        {
            var key = await _context.ApiKeys.AsNoTracking().
                FirstOrDefaultAsync(k => k.Key == apiKey, token);
            return (key != null && key.IsActive);
        }
    }
}
