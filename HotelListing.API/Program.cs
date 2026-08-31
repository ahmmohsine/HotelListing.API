using HotelListing.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container using grouped extension method
builder.Services.AddApiServices();

var app = builder.Build();

// Configure the HTTP request pipeline using grouped extension method
app.UseApiConfiguration();

app.Run();

// Partial Program class to support WebApplicationFactory<Program> integration testing
public partial class Program { }
