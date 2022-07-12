using CentOps.Api.Authentication;
using CentOps.Api.Authentication.Extensions;
using CentOps.Api.Configuration;
using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Azure.Cosmos;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        [ExcludeFromCodeCoverage]
        public static void AddApiKeyAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddSingleton(new AuthConfig
            {
                AdminApiKey = configuration.GetConnectionString("AdminApiKey")
            });

            _ = services.AddAuthentication()
                .AddApiKeyAuth<AdminApiUserClaimsProvider>(ApiKeyAuthenticationDefaults.AdminAuthenticationScheme)
                .AddApiKeyAuth<ApiUserClaimsProvider>(ApiKeyAuthenticationDefaults.AuthenticationScheme);
        }

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            _ = services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));

            _ = services.AddAuthorization(options =>
            {
                var adminPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AdminAuthenticationScheme)
                    .RequireClaim("admin", bool.TrueString)
                    .Build();

                options.AddPolicy(AuthConfig.AdminPolicy, adminPolicy);

                var participantPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AuthenticationScheme)
                    .RequireClaim("id")
                    .Build();

                options.AddPolicy(AuthConfig.ParticipantPolicy, participantPolicy);
            });
        }

        public static void AddDataStore(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException(nameof(configurationManager));
            }

            var features = configurationManager.GetSection("FeatureToggles").Get<FeatureToggles>();
            if (features != null && features.UseInMemoryStore)
            {
                services.AddInMemoryDataStores();
            }
            else
            {
                var section = configurationManager.GetSection("CosmosDb");
                services.AddCosmosDbServices(section);
            }
        }

        public static void AddInMemoryDataStores(this IServiceCollection services)
        {
            Trace.WriteLine("Using InMemory DataStore.");
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

            Trace.WriteLine("Using CosmosDb DataStore.");
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
