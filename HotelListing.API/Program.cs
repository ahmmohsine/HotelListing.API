using HotelListing.API.Data;
using HotelListing.API.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices(builder.Configuration);
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HotelListingDbContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedDataAsync(context);

}


app.UseApiConfiguration();

app.Run();

public partial class Program { }
