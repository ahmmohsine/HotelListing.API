using HotelListing.API.DTOs.Country;
using HotelListing.API.Results;

namespace HotelListing.API.Services;

public interface ICountryService
{
    Task<Result<IEnumerable<CountryReadOnlyDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<CountryReadOnlyDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<CountryReadOnlyDto>> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Result<CountryReadOnlyDto>> CreateCountryAsync(CreateCountryDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(int id, UpdateCountryDto dto, CancellationToken ct = default);
    Task<Result> DeleteCountryAsync(int id, CancellationToken ct = default);
}