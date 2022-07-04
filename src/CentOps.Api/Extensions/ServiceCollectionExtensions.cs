using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Extensions
{
    [ExcludeFromCodeCoverage] // This is not solution code, no need for unit tests
    public static class ServiceCollectionExtensions
    {
        public static void AddCosmosDbServices(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }

            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];

            var cosmosClient = new CosmosClient(account, key);
            var cosmosDbService = new CosmosDbService(cosmosClient, databaseName, containerName);

            _ = services.AddSingleton(_ =>
            {
                var cosmosClient = new CosmosClient(account, key);
                return new CosmosDbService(cosmosClient, databaseName, containerName);
            });

            _ = services.AddSingleton<IInstitutionStore>(provider => provider.GetRequiredService<CosmosDbService>());
            _ = services.AddSingleton<IParticipantStore>(provider => provider.GetRequiredService<CosmosDbService>());
        }
    }
}
