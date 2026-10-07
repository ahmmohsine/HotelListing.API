using HotelListing.API.Constants;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Handlers;
using HotelListing.API.Repositories;
using HotelListing.API.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

            services.AddIdentityApiEndpoints<ApplicationUser>()
                  .AddRoles<IdentityRole>().
                AddEntityFrameworkStores<HotelListingDbContext>();
            ;

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = AuthenticationDefaults.ApiKeyScheme;
            //    options.DefaultChallengeScheme = AuthenticationDefaults.ApiKeyScheme;
            //}
            //).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { }).
            //AddScheme<AuthenticationSchemeOptions, ApiAuthenticationHandler>(AuthenticationDefaults.ApiKeyScheme, _ => { });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).
            AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                    (System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero for immediate expiration default is 5min
                };
            }).
            AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { }).
            AddScheme<AuthenticationSchemeOptions, ApiAuthenticationHandler>(AuthenticationDefaults.ApiKeyScheme, _ => { });
            // (options => options.Password.RequiredLength = 5);
            services.AddAuthorization();
            // Controllers and API behavior
            services.AddControllers();

            // OpenAPI / Swagger documentation
            services.AddOpenApi();

            // Repositories
            services.AddScoped<IGenericRepository<Country>, CountryRepository>();
            services.AddScoped<IGenericRepository<Hotel>, HotelRepository>();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IApiKeyValidatorService, ApiKeyValidatorService>();

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