using HotelListing.API.Data;
using HotelListing.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container using grouped extension method
builder.Services.AddApiServices(builder.Configuration);

//builder.Services.
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HotelListingDbContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedDataAsync(context);

}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.
        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});
// Configure the HTTP request pipeline using grouped extension method
app.UseApiConfiguration();

app.Run();

// Partial Program class to support WebApplicationFactory<Program> integration testing
public partial class Program { }
