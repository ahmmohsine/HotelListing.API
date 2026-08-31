using HotelListing.API.DTOs;

namespace HotelListing.API.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryReadOnlyDto>> GetAllAsync(CancellationToken ct = default);
        Task<CountryReadOnlyDto?> GetByIsoCodeAsync(string code, CancellationToken ct = default);
    }
}
