using HotelListing.API.Data;
using HotelListing.API.Extensions;
using Microsoft.EntityFrameworkCore;

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

// Configure the HTTP request pipeline using grouped extension method
app.UseApiConfiguration();

app.Run();

// Partial Program class to support WebApplicationFactory<Program> integration testing
public partial class Program { }
