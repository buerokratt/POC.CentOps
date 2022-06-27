
using CentOps.Api.Authentication;

namespace CentOps.Api.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            _ = services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", builder =>
                {
                    _ = builder
                        .AddAuthenticationSchemes(ApiKeyAuthenciationDefaults.AuthenticationScheme)
                        .RequireClaim("isAdmin", "TRUE");
                });

                options.AddPolicy("UserPolicy", builder =>
                {
                    _ = builder
                        .AddAuthenticationSchemes(ApiKeyAuthenciationDefaults.AuthenticationScheme)
                        .RequireClaim("id");
                });

                options.DefaultPolicy = options.GetPolicy("UserPolicy") ?? throw new InvalidOperationException();
            });
        }
    }
}
