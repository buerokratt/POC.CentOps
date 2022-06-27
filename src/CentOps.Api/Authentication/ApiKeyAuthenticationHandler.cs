using CentOps.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CentOps.Api.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IAuthService _userService;

        public ApiKeyAuthenticationHandler(
            IAuthService userService,
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) :
            base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKey = ExtractApiKey(Request, Options.ApiKeyHeaderName);

            if (string.IsNullOrEmpty(apiKey))
            {
                return AuthenticateResult.Fail("The 'Authorization' header does not contain the API Key.");
            }

            var user = await _userService.AuthenticateAsync(apiKey).ConfigureAwait(false);

            if (user == null)
            {
                return AuthenticateResult.Fail("The API is invalid.");
            }

            var claims = new[]
            {
                new Claim("id", user.Id),
                new Claim("isAdmin", user.IsAdmin.ToString().ToUpperInvariant())
            };

            var identity = new ClaimsIdentity(claims, ApiKeyAuthenciationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenciationDefaults.AuthenticationScheme);

            return AuthenticateResult.Success(ticket);
        }

        private static string? ExtractApiKey(HttpRequest request, string headerName)
        {
            return request.Headers.TryGetValue(headerName, out var value)
                ? (string)value
                : null;
        }
    }
}