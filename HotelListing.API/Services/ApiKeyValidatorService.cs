using HotelListing.API.Contracts;

namespace HotelListing.API.Services
{
    //If use DB to store API keys, must inject DB context
    public class ApiKeyValidatorService(IConfiguration configuration) : IApiKeyValidatorService
    {
        public Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken token = default)
        {
            //Query to table to check if the API key exists and is valid and if experation date is not passed
            //return true or false based on the result of the query
            return Task.FromResult(apiKey.Equals(configuration["ApiKey"]));
        }
    }
}
