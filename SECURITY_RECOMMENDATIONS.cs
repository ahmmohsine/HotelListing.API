// RECOMMENDED SECURITY ENHANCEMENTS FOR ServiceCollectionExtensions.cs
// This file shows the recommended additions to enhance API security
// 
// Add these using statements:
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.RateLimiting;
// using System.Security.Cryptography;
// using System.Text;

namespace HotelListing.API.Extensions.Recommended
{
    /// <summary>
    /// RECOMMENDED: Enhanced ServiceCollectionExtensions with security best practices.
    /// This example shows how to implement:
    /// - JWT Authentication
    /// - CORS Policy
    /// - Input Validation
    /// - Rate Limiting
    /// - Security Headers
    /// </summary>
    public static class EnhancedServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServicesWithSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ============================================================================
            // 1. ADD AUTHENTICATION (JWT)
            // ============================================================================
            // NOTE: Store JWT secret in environment variables or Azure Key Vault, NOT appsettings
            var jwtSecret = configuration["Jwt:Secret"] 
                ?? throw new InvalidOperationException("JWT Secret not configured");
            var jwtIssuer = configuration["Jwt:Issuer"] 
                ?? throw new InvalidOperationException("JWT Issuer not configured");
            var jwtAudience = configuration["Jwt:Audience"] 
                ?? throw new InvalidOperationException("JWT Audience not configured");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSecret)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Log authentication failures for security monitoring
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogWarning("JWT authentication failed: {Error}", context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsJsonAsync(new
                            {
                                error = "Unauthorized",
                                message = "Valid authentication is required"
                            });
                        }
                    };
                });

            // ============================================================================
            // 2. ADD CORS POLICY (Restrict to known origins)
            // ============================================================================
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
                {
                    var allowedOrigins = configuration.GetSection("CorsOrigins").Get<string[]>()
                        ?? throw new InvalidOperationException("CorsOrigins not configured");

                    policyBuilder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("X-Total-Count"); // For pagination
                });
            });

            // ============================================================================
            // 3. ADD RATE LIMITING
            // ============================================================================
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,           // 100 requests
                            Window = TimeSpan.FromMinutes(1), // per minute
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 2
                        }));

                // Stricter limits for auth endpoints
                options.AddPolicy("strict", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));
            });

            // ============================================================================
            // 4. ADD AUTHORIZATION POLICIES
            // ============================================================================
            services.AddAuthorizationBuilder()
                .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
                .AddPolicy("User", policy => policy.RequireRole("User", "Admin"));

            // ============================================================================
            // 5. REGISTER CORE SERVICES
            // ============================================================================
            services.AddControllers();
            services.AddOpenApi();

            // Repository registrations
            services.AddScoped<ICountryRepository, CountryRepository>();

            // ============================================================================
            // 6. ADD LOGGING
            // ============================================================================
            services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
                // Consider adding Serilog for structured logging
                // config.AddSerilog();
            });

            return services;
        }
    }
}

// ============================================================================
// RECOMMENDED: Updated ApplicationBuilderExtensions with security middleware
// ============================================================================
namespace HotelListing.API.Extensions.Recommended
{
    public static class EnhancedApplicationBuilderExtensions
    {
        public static WebApplication UseApiConfigurationWithSecurity(
            this WebApplication app,
            ILogger<Program> logger)
        {
            logger.LogInformation("Configuring API security middleware...");

            // ============================================================================
            // 1. EXCEPTION HANDLING (Before other middleware)
            // ============================================================================
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                    };

                    // Log the actual error internally, but don't expose stack trace to client
                    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                    var innerLogger = loggerFactory.CreateLogger("ExceptionHandler");
                    innerLogger.LogError("Unhandled exception occurred");

                    if (!app.Environment.IsProduction())
                    {
                        // Only expose details in development
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        problemDetails.Detail = exceptionHandlerFeature?.Error.ToString();
                    }

                    await context.Response.WriteAsJsonAsync(problemDetails);
                });
            });

            // ============================================================================
            // 2. SECURITY HEADERS
            // ============================================================================
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

                if (!app.Environment.IsDevelopment())
                {
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                }

                await next();
            });

            // ============================================================================
            // 3. DEVELOPMENT-ONLY OPENAPI
            // ============================================================================
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                logger.LogInformation("OpenAPI documentation enabled (Development only)");
            }

            // ============================================================================
            // 4. HTTPS REDIRECTION
            // ============================================================================
            app.UseHttpsRedirection();

            // ============================================================================
            // 5. CORS (Must be before Auth middleware)
            // ============================================================================
            app.UseCors("AllowSpecificOrigins");
            logger.LogInformation("CORS policy applied");

            // ============================================================================
            // 6. HSTS (HTTP Strict Transport Security)
            // ============================================================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            // ============================================================================
            // 7. RATE LIMITING (Before Auth)
            // ============================================================================
            app.UseRateLimiter();

            // ============================================================================
            // 8. AUTHENTICATION (Must be before Authorization)
            // ============================================================================
            app.UseAuthentication();
            logger.LogInformation("Authentication middleware enabled");

            // ============================================================================
            // 9. AUTHORIZATION
            // ============================================================================
            app.UseAuthorization();
            logger.LogInformation("Authorization middleware enabled");

            // ============================================================================
            // 10. ROUTING
            // ============================================================================
            app.MapControllers();

            logger.LogInformation("API security middleware configured successfully");

            return app;
        }
    }
}

// ============================================================================
// RECOMMENDED: appsettings.json with security configuration
// ============================================================================
/*
{
  "Jwt": {
    "Secret": "STORE_IN_ENVIRONMENT_VARIABLES_NOT_HERE",
    "Issuer": "your-api-issuer",
    "Audience": "your-api-audience",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "CorsOrigins": [
    "https://yourfrontend.com",
    "https://staging-frontend.com"
  ],
  "AllowedHosts": "yourdomain.com,api.yourdomain.com",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Authentication": "Information"
    }
  }
}
*/

// ============================================================================
// RECOMMENDED: Program.cs example using enhanced services
// ============================================================================
/*
using HotelListing.API.Extensions;
using HotelListing.API.Extensions.Recommended;

var builder = WebApplication.CreateBuilder(args);

// Add services with enhanced security
builder.Services.AddApiServicesWithSecurity(builder.Configuration);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Configure pipeline with enhanced security
app.UseApiConfigurationWithSecurity(logger);

app.Run();

public partial class Program { }
*/
