using Bogus;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

public static class Seed
{
    public static async Task SeedDataAsync(HotelListingDbContext context, CancellationToken ct = default)
    {
        await SeedCountriesAsync(context, ct);
        await SeedHotelsAsync(context, ct);
    }

    private static async Task SeedCountriesAsync(HotelListingDbContext context, CancellationToken ct)
    {
        if (await context.Countries.AnyAsync(ct)) return;

        // Note : On ne définit PAS d'Id ici pour laisser la BDD gérer l'auto-incrément
        var countryFaker = new Faker<Country>("fr")
            .RuleFor(c => c.Name, f => f.Address.Country())
            .RuleFor(c => c.Code, f => f.Address.CountryCode());

        var countries = countryFaker.Generate(10);
        await context.Countries.AddRangeAsync(countries, ct);
        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedHotelsAsync(HotelListingDbContext context, CancellationToken ct)
    {
        if (await context.Hotels.AnyAsync(ct)) return;

        var validCountryIds = await context.Countries
            .Select(c => c.Id)
            .ToListAsync(ct);

        if (!validCountryIds.Any()) return;

        var hotelFaker = new Faker<Hotel>("fr")
            .RuleFor(h => h.Name, f => $"{f.Company.CompanyName()} Hotel")
            .RuleFor(h => h.Address, f => f.Address.FullAddress())
            .RuleFor(h => h.Rating, f => Math.Round(f.Random.Double(1, 5), 1))
            .RuleFor(h => h.CountryId, f => f.PickRandom(validCountryIds));

        var hotels = hotelFaker.Generate(20);
        await context.Hotels.AddRangeAsync(hotels, ct);
        await context.SaveChangesAsync(ct);
    }
}