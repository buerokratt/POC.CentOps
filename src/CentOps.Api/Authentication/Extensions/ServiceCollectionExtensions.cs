namespace CentOps.Api.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApiKeyAuthentication(this IServiceCollection services)
        {
            _ = services
                .AddAuthentication()
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenciationDefaults.AuthenticationScheme,
                    null);
        }
    }
}
