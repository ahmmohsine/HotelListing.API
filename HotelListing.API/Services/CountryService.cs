using HotelListing.API.Data;
using HotelListing.API.DTOs.Country;
using HotelListing.API.Repositories;
using HotelListing.API.Results;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Services;

public class CountryService : ICountryService
{
    private readonly IGenericRepository<Country> _countryRepository;
    private readonly IMapper _mapper;

    public CountryService(IGenericRepository<Country> countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<IEnumerable<CountryReadOnlyDto>>> GetAllAsync(CancellationToken ct = default)
    {
        // Projection SQL directe (optimisation mémoire)
        var countries = await _countryRepository.GetQueryable()
            .ProjectToType<CountryReadOnlyDto>(_mapper.Config)
            .ToListAsync(ct);

        return Result<IEnumerable<CountryReadOnlyDto>>.Success(countries);
    }

    public async Task<Result<CountryReadOnlyDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var country = await _countryRepository.GetByIdAsync(id, ct);
        if (country is null)
            return Result<CountryReadOnlyDto>.Failure(new Error("Country.NotFound", $"Le pays avec l'ID {id} n'a pas été trouvé."));

        return Result<CountryReadOnlyDto>.Success(_mapper.Map<CountryReadOnlyDto>(country));
    }

    public async Task<Result<CountryReadOnlyDto>> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var country = await _countryRepository.FindAsync(c => c.Code == code, ct);
        if (country is null)
            return Result<CountryReadOnlyDto>.Failure(new Error("Country.NotFound", $"Le pays avec le code {code} n'a pas été trouvé."));

        return Result<CountryReadOnlyDto>.Success(_mapper.Map<CountryReadOnlyDto>(country));
    }

    public async Task<Result<CountryReadOnlyDto>> CreateCountryAsync(CreateCountryDto dto, CancellationToken ct = default)
    {
        // Validation d'unicité métier
        bool codeExists = await _countryRepository.GetQueryable().AnyAsync(c => c.Code == dto.Code, ct);
        if (codeExists)
            return Result<CountryReadOnlyDto>.Failure(new Error("Country.DuplicateCode", $"Le code pays '{dto.Code}' est déjà utilisé."));

        var country = _mapper.Map<Country>(dto);
        var createdCountry = await _countryRepository.AddAsync(country, ct);

        return Result<CountryReadOnlyDto>.Success(_mapper.Map<CountryReadOnlyDto>(createdCountry));
    }

    public async Task<Result> UpdateAsync(int id, UpdateCountryDto dto, CancellationToken ct = default)
    {
        var country = await _countryRepository.GetByIdAsync(id, ct);
        if (country is null)
            return Result.Failure(new Error("Country.NotFound", $"Impossible de mettre à jour : le pays {id} n'existe pas."));

        // Application des modifications sur l'entité suivie par ChangeTracker
        _mapper.Map(dto, country);

        await _countryRepository.UpdateAsync(country, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteCountryAsync(int id, CancellationToken ct = default)
    {
        var country = await _countryRepository.GetByIdAsync(id, ct);
        if (country is null)
            return Result.Failure(new Error("Country.NotFound", $"Impossible de supprimer : le pays {id} n'existe pas."));

        await _countryRepository.DeleteAsync(country.Id, ct);
        return Result.Success();
    }
}