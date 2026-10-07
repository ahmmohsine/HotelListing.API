namespace HotelListing.API.Contracts
{
    public interface IApiKeyRepository
    {
        Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken token = default);
    }
}