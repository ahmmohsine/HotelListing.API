using HotelListing.API.Data;
using HotelListing.API.Repositories;
using HotelListing.API.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Extensions
{
    /// <summary>
    /// Extension methods for configuring API services.
    /// Groups related service registrations for improved maintainability.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds core API services to the dependency injection container.
        /// Registers controllers, OpenAPI documentation, and repository implementations.
        /// </summary>
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            var connectionString = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetConnectionString("HotelListingDbConnectionString");
            services.AddDbContext<HotelListingDbContext>(options => options.UseSqlServer(connectionString));
            // Controllers and API behavior
            services.AddControllers();

            // OpenAPI / Swagger documentation
            services.AddOpenApi();

            // Repository implementations
            services.AddScoped<IGenericRepository<Country>, CountryRepository>();
            services.AddScoped<IGenericRepository<Hotel>, HotelRepository>();

            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<ICountryService, CountryService>();

            return services;
        }
    }
}
