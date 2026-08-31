using Microsoft.AspNetCore.Builder;

namespace HotelListing.API.Extensions
{
    /// <summary>
    /// Extension methods for configuring the HTTP request pipeline.
    /// Groups middleware configuration for improved maintainability.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the HTTP request pipeline for the API.
        /// Sets up middleware in the correct order: development-specific handlers, HTTPS, authorization, and routing.
        /// </summary>
        public static WebApplication UseApiConfiguration(this WebApplication app)
        {
            // Configure development-specific middleware
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Enforce HTTPS
            app.UseHttpsRedirection();

            // Authorization middleware
            app.UseAuthorization();

            // Map controller endpoints
            app.MapControllers();

            return app;
        }
    }
}
