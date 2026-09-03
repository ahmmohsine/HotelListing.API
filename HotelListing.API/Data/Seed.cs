using Bogus;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{
    public class Seed
    {
        public static async Task SeedHotelsAsync(HotelListingDbContext context, CancellationToken cancellationToken = default)
        {
            if (await context.Hotels.AnyAsync(cancellationToken)) return;

            var hotelFaker = new Faker<Hotel>()
                .RuleFor(h => h.Name, f => f.Company.CompanyName() + " Hotel")
                .RuleFor(h => h.Address, f => f.Address.FullAddress())
                .RuleFor(h => h.Rating, f => Math.Round(f.Random.Double(1, 5), 1))
                .RuleFor(h => h.CountryId, f => f.Random.Number(1, 5));
            var hotels = hotelFaker.Generate(10);

            await context.Hotels.AddRangeAsync(hotels, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        public static async Task GenerateCountries(HotelListingDbContext context, CancellationToken cancellationToken = default)
        {
            if (await context.Countries.AnyAsync(cancellationToken)) return;

            var countryFaker = new Faker<Country>("fr")
           .RuleFor(c => c.Id, f => f.IndexGlobal)
           .RuleFor(c => c.Name, f => f.Address.Country())
           .RuleFor(c => c.Code, f => f.Address.CountryCode());

            var countries = countryFaker.Generate(10);

            await context.Countries.AddRangeAsync(countries, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        public static List<Hotel> GenerateHotels_(int count = 10)
        {
            var hotelId = 1;

            var hotelFaker = new Faker<Hotel>()
                .RuleFor(h => h.Id, f => hotelId++)
                .RuleFor(h => h.Name, f => $"{f.Company.CompanyName()} Hotel")
                .RuleFor(h => h.Address, f => f.Address.FullAddress())
                .RuleFor(h => h.Rating, f => Math.Round(f.Random.Double(1, 5), 1))
                .RuleFor(h => h.CountryId, f => f.Random.Int(1, 3))
                ;

            return hotelFaker.Generate(count);
        }
        public static List<Country> GenerateCountries(int count = 10)
        {
            var countryFaker = new Faker<Country>("fr")
             .RuleFor(c => c.Id, f => f.IndexGlobal)
             .RuleFor(c => c.Name, f => f.Address.Country())
             .RuleFor(c => c.Code, f => f.Address.CountryCode());

            return countryFaker.Generate(count);
        }
    }
}