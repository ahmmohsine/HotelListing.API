using HotelListing.API.Data;
using HotelListing.API.Repositories;
using HotelListing.API.Services;
using Mapster;
using MapsterMapper;
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
        /// </summary>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("HotelListingDbConn");

            services.AddDbContext<HotelListingDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Controllers and API behavior
            services.AddControllers();

            // OpenAPI / Swagger documentation
            services.AddOpenApi();

            // Repositories
            services.AddScoped<IGenericRepository<Country>, CountryRepository>();
            services.AddScoped<IGenericRepository<Hotel>, HotelRepository>();

            // Domain Services
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<ICountryService, CountryService>();

            // 1. Charger la configuration globale de Mapster
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(Program).Assembly);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            return services;
        }
    }
}