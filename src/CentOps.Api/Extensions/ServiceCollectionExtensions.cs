using CentOps.Api.Authentication;
using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Azure.Cosmos;

namespace CentOps.Api.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            _ = services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));

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
        }

        public static void AddCosmosDbServices(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }

            _ = services.AddSingleton(_ =>
            {
                var databaseName = configurationSection["DatabaseName"];
                var containerName = configurationSection["ContainerName"];
                var account = configurationSection["Account"];
                var key = configurationSection["Key"];

                var cosmosClient = new CosmosClient(account, key);
                return new CosmosDbService(cosmosClient, databaseName, containerName);
            });

            _ = services.AddSingleton<IInstitutionStore>(provider => provider.GetRequiredService<CosmosDbService>());
            _ = services.AddSingleton<IParticipantStore>(provider => provider.GetRequiredService<CosmosDbService>());
        }
    }
}
