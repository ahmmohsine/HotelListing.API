using Bogus;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Services;

public class IdentitySeeder : ISeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly HotelListingDbContext _context;

    public IdentitySeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        HotelListingDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // 1. Toujours exécuter les migrations EN PREMIER
        await _context.Database.MigrateAsync(ct);

        // 2. Initialiser les Rôles et Utilisateurs
        await SeedRolesAndUsersAsync(ct);

        // 3. Initialiser les Données Métier (Pays & Hôtels)
        await SeedCountriesAsync(ct);
        await SeedHotelsAsync(ct);
    }

    private async Task SeedRolesAndUsersAsync(CancellationToken ct)
    {
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Création de l'Administrateur principal
        await SeedAdminUserAsync();

        // Création des Utilisateurs Fictifs avec Bogus
        // await SeedUsersAsync(count: 10, ct);
    }

    private async Task SeedAdminUserAsync()
    {
        const string adminEmail = "admin@hotellisting.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newAdmin, "AdminPassword123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }

    /*  private async Task SeedUsersAsync(int count, CancellationToken ct)
      {
          // Idempotence : Ne pas régénérer si des utilisateurs existent déjà
          if (await _context.Users.AnyAsync(u => u.Email != "admin@hotellisting.com", ct))
          {
              return;
          }

          var faker = new Faker<ApplicationUser>("fr")
              .RuleFor(u => u.FirstName, f => f.Name.FirstName())
              .RuleFor(u => u.LastName, f => f.Name.LastName())
              .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
              .RuleFor(u => u.UserName, (f, u) => u.Email)
              .RuleFor(u => u.EmailConfirmed, true);

          var fakeUsers = faker.Generate(count);
          const string defaultPassword = "Password123!";

          foreach (var user in fakeUsers)
          {
              if (ct.IsCancellationRequested) break;

              var result = await _userManager.CreateAsync(user, defaultPassword);
              if (result.Succeeded)
              {
                  await _userManager.AddToRoleAsync(user, "User");
              }
          }
      }
  */
    private async Task SeedCountriesAsync(CancellationToken ct)
    {
        if (await _context.Countries.AnyAsync(ct)) return;

        var countryFaker = new Faker<Country>("fr")
            .RuleFor(c => c.Name, f => f.Address.Country())
            .RuleFor(c => c.Code, f => f.Address.CountryCode());

        var countries = countryFaker.Generate(10);
        await _context.Countries.AddRangeAsync(countries, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedHotelsAsync(CancellationToken ct)
    {
        if (await _context.Hotels.AnyAsync(ct)) return;

        var validCountryIds = await _context.Countries
            .Select(c => c.Id)
            .ToListAsync(ct);

        if (!validCountryIds.Any()) return;

        var hotelFaker = new Faker<Hotel>("fr")
            .RuleFor(h => h.Name, f => $"{f.Company.CompanyName()} Hotel")
            .RuleFor(h => h.Address, f => f.Address.FullAddress())
            .RuleFor(h => h.Rating, f => Math.Round(f.Random.Double(1, 5), 1))
            .RuleFor(h => h.CountryId, f => f.PickRandom(validCountryIds));

        var hotels = hotelFaker.Generate(20);
        await _context.Hotels.AddRangeAsync(hotels, ct);
        await _context.SaveChangesAsync(ct);
    }
}