using HotelListing.API.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace HotelListing.API.Handlers;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IUserService userService) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private const string Prefix = "Basic ";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var authHeader = authHeaderValues.ToString();
        if (!authHeader.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        string[] credentials;
        try
        {
            var bytes = Convert.FromBase64String(authHeader[Prefix.Length..].Trim());
            credentials = Encoding.UTF8.GetString(bytes).Split(':', 2);
        }
        catch (FormatException)
        {
            return AuthenticateResult.Fail("Invalid Authorization header.");
        }

        if (credentials.Length != 2
            || string.IsNullOrWhiteSpace(credentials[0])
            || string.IsNullOrEmpty(credentials[1]))
        {
            return AuthenticateResult.Fail("Invalid Authorization header.");
        }

        var email = credentials[0];
        var password = credentials[1];

        // Method to create: returns null if the credentials are wrong,
        // otherwise a user with Id, Email and Roles. It must NOT generate a JWT.
        var user = await userService.ValidateCredentialsAsync(email, password);
        if (user is null)
        {
            return AuthenticateResult.Fail("Invalid credentials.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Email)
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"HotelListing API\", charset=\"UTF-8\"";
        return base.HandleChallengeAsync(properties);
    }
}
