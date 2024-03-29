﻿using CentOps.Api.Authentication.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CentOps.Api.Authentication
{
    public class ApiKeyAuthenticationHandler<TClaimsProvider> : AuthenticationHandler<ApiKeyAuthenticationOptions>
        where TClaimsProvider : class, IApiUserClaimsProvider
    {
        private readonly TClaimsProvider _claimsProvider;

        public ApiKeyAuthenticationHandler(
            TClaimsProvider claimsProvider,
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) :
            base(options, logger, encoder, clock)
        {
            _claimsProvider = claimsProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKey = ExtractApiKey(Request, Options.ApiKeyHeaderName);

            if (string.IsNullOrEmpty(apiKey))
            {
                return AuthenticateResult.Fail($"The '{Options.ApiKeyHeaderName}' header does not contain a value.");
            }

            var apiUser = await _claimsProvider.GetUserClaimsAsync(apiKey).ConfigureAwait(false);

            if (apiUser == null)
            {
                return AuthenticateResult.Fail($"The '{Options.ApiKeyHeaderName}' header contains an invalid API key.");
            }

            var identity = new ClaimsIdentity(apiUser.Claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

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