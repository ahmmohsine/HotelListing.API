namespace HotelListing.API.Contracts
{
    public interface ISeeder
    {
        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}
