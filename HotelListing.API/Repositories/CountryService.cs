using HotelListing.API.DTOs;
using HotelListing.API.Services;

namespace HotelListing.API.Repositories
{
    internal class CountryService : ICountryService
    {
        public Task<IEnumerable<CountryReadOnlyDto>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CountryReadOnlyDto?> GetByIsoCodeAsync(string code, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}