using Bogus;

namespace HotelListing.API.Data
{
    public class Seed
    {
        /* public static async Task SeedHotelsAsync(AppDbContext context, CancellationToken cancellationToken = default)
         {
             // 1. Vérification d'existence préalable (éviter les doublons)
             if (await context.Hotels.AnyAsync(cancellationToken)) return;

             // 2. Configuration du Generator Bogus pour la classe Hotel
             var hotelFaker = new Faker<Hotel>()
                 .RuleFor(h => h.Name, f => f.Company.CompanyName() + " Hotel")
                 .RuleFor(h => h.Address, f => f.Address.FullAddress())
                 .RuleFor(h => h.Rating, f => Math.Round(f.Random.Double(1, 5), 1))
                 .RuleFor(h => h.CountryId, f => f.Random.Number(1, 5)); // Ajuster selon tes IDs de pays existants

             // 3. Génération synchrone en mémoire
             var hotels = hotelFaker.Generate(10);

             // 4. Insertion asynchrone groupée
             await context.Hotels.AddRangeAsync(hotels, cancellationToken);
             await context.SaveChangesAsync(cancellationToken);
         }*/
        public static List<Hotel> GenerateHotels(int count = 10)
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
    }
}