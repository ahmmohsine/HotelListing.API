namespace HotelListing.API.Contracts
{
    public interface IApiKeyValidatorService
    {
        Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken token = default);
    }
}