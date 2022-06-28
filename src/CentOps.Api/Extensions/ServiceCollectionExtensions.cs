using CentOps.Api.Authentication;
using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CentOps.Api.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            _ = services.AddAuthorization(options =>
            {
                var builder = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(ApiKeyAuthenciationDefaults.AuthenticationScheme)
                    .RequireClaim("id");

                var userPolicy = builder.Build();

                options.AddPolicy("UserPolicy", userPolicy);
                options.AddPolicy("AdminPolicy", options =>
                {
                    _ = options
                        .Combine(userPolicy)
                        .RequireClaim("isAdmin", "TRUE");
                });

                options.DefaultPolicy = userPolicy;
            });
        }

        public static void AddDataStores(this IServiceCollection services)
        {
            var inMemoryStore = new InMemoryStore();
            _ = services.AddSingleton<IInstitutionStore>(provider => inMemoryStore);
            _ = services.AddSingleton<IParticipantStore>(provider => inMemoryStore);
            _ = services.AddSingleton<IApiUserStore>(provider => inMemoryStore);
        }
    }
}
