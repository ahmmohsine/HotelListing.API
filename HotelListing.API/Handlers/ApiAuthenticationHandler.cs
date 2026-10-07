using HotelListing.API.Constants;
using HotelListing.API.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HotelListing.API.Handlers;

public class ApiAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyValidatorService apiKeyValidatorService) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private const string Prefix = "Api ";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKey = string.Empty;
        if (Request.Headers.TryGetValue(AuthenticationDefaults.ApiKeyHeaderName, out var headerValue))
        {
            apiKey = headerValue.ToString();
        }
        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.NoResult();
        }
        var valid = await apiKeyValidatorService.
            ValidateApiKeyAsync(apiKey, Context.RequestAborted);

        if (!valid)
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }


        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "ApiKeyUser"),
            new Claim(ClaimTypes.NameIdentifier, "ApiKey")
        };



        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

}