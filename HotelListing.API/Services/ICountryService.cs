using HotelListing.API.DTOs;

namespace HotelListing.API.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryReadOnlyDto>> GetAllAsync(CancellationToken ct = default);
        Task<CountryReadOnlyDto?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<CountryReadOnlyDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateCountryDto dto, CancellationToken ct = default);
        Task<CountryReadOnlyDto> CreateCountryAsync(CreateCountryDto dto, CancellationToken ct = default);
        Task<bool> DeleteCountryAsync(int id, CancellationToken ct = default);
    }
}
