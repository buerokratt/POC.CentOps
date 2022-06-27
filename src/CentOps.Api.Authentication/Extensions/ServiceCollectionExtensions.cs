using CentOps.Api.Authentication.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CentOps.Api.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApiKeyAuthentication<TApiKeyClaimsProvider>(
            this IServiceCollection services,
            string? apiKeyHeaderName = null)
            where TApiKeyClaimsProvider : class, IApiUserClaimsProvider
        {
            if (string.IsNullOrEmpty(apiKeyHeaderName))
            {
                apiKeyHeaderName = ApiKeyAuthenciationDefaults.DefaultApiKeyHeaderName;
            }

            _ = services.AddSingleton<IApiUserClaimsProvider, TApiKeyClaimsProvider>();

            _ = services
                .AddAuthentication()
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenciationDefaults.AuthenticationScheme,
                    null);
        }
    }
}
