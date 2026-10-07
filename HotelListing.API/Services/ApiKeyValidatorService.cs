using HotelListing.API.Contracts;

namespace HotelListing.API.Services
{
    //If use DB to store API keys, must inject DB context
    public class ApiKeyValidatorService : IApiKeyValidatorService
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public ApiKeyValidatorService(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken token = default)
        {
            return await _apiKeyRepository.ValidateApiKeyAsync(apiKey, token);
        }
    }
}
