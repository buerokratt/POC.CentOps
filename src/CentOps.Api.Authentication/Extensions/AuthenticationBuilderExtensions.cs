using CentOps.Api.Authentication.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Authentication.Extensions
{
    [ExcludeFromCodeCoverage] // The ApiKeyAuthenticationHandler tests some of the ApiKeyAuthenticationOptions related functionality.
    public static class AuthenticationBuilderExtensions
    {
        public static void AddApiKeyAuth<TClaimsProvider>(this AuthenticationBuilder builder, string? apiKeyHeaderName = null)
            where TClaimsProvider : class, IApiUserClaimsProvider
        {
            _ = builder.Services.AddSingleton<IApiUserClaimsProvider, TClaimsProvider>();

            _ = builder
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenciationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.ApiKeyHeaderName = string.IsNullOrEmpty(apiKeyHeaderName)
                            ? ApiKeyAuthenciationDefaults.DefaultApiKeyHeaderName
                            : apiKeyHeaderName;
                    });
        }
    }
}
