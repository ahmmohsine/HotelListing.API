using HotelListing.API.Data;
using HotelListing.API.DTOs.Country;
using HotelListing.API.Repositories;

namespace HotelListing.API.Services;

public class CountryService(IGenericRepository<Country> _countryRepository) : ICountryService
{
    public async Task<IEnumerable<CountryReadOnlyDto>> GetAllAsync(CancellationToken ct = default)
    {
        var countries = await _countryRepository.GetAllAsync(ct);
        return countries.Select(ToCountryReadOnlyDto).ToList();
    }

    public async Task<CountryReadOnlyDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var country = await _countryRepository.GetByIdAsync(id, ct);
        if (country == null) return null;

        return ToCountryReadOnlyDto(country);
    }

    public async Task<CountryReadOnlyDto?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var country = await _countryRepository.FindAsync(c => c.Code == code, ct);
        if (country == null) return null;

        return ToCountryReadOnlyDto(country);
    }

    public async Task<CountryReadOnlyDto> CreateCountryAsync(CreateCountryDto dto, CancellationToken ct = default)
    {
        var country = ToCountry(dto);
        var createdCountry = await _countryRepository.AddAsync(country, ct);
        return ToCountryReadOnlyDto(createdCountry);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCountryDto dto, CancellationToken ct = default)
    {
        var country = await _countryRepository.GetByIdAsync(id, ct);
        if (country == null) return false;

        country.Name = dto.Name;
        country.Code = dto.Code;

        await _countryRepository.UpdateAsync(country, ct);
        return true;
    }

    public async Task<bool> DeleteCountryAsync(int id, CancellationToken ct = default)
    {
        var exists = await _countryRepository.ExistsAsync(id, ct);
        if (!exists) return false;

        await _countryRepository.DeleteAsync(id, ct);
        return true;
    }


    private static CountryReadOnlyDto ToCountryReadOnlyDto(Country country)
    {
        return new CountryReadOnlyDto
        {
            Id = country.Id,
            Name = country.Name,
            Code = country.Code
        };
    }

    private static Country ToCountry(CreateCountryDto dto)
    {
        return new Country
        {
            Name = dto.Name,
            Code = dto.Code
        };
    }
}