using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace CentOps.Api
{
    [ExcludeFromCodeCoverage] // This is not solution code, no need for unit tests
    public static class Program
    {
#pragma warning disable CA1506
        public static void Main(string[] args)
#pragma warning restore CA1506
        {
            var builder = WebApplication.CreateBuilder(args);

            _ = builder.Services.AddControllers().AddJsonOptions(jo =>
            {
                jo.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            _ = builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            // var inMemoryStore = new InMemoryStore();
            // _ = builder.Services.AddSingleton<IInstitutionStore>(provider => inMemoryStore);
            // _ = builder.Services.AddSingleton<IParticipantStore>(provider => inMemoryStore);

            var section = builder.Configuration.GetSection("CosmosDb");
            var cosmosDb = InitializeCosmosClientInstanceAsync(section).GetAwaiter().GetResult();
            _ = builder.Services.AddSingleton<IInstitutionStore>(provider => cosmosDb);
            _ = builder.Services.AddSingleton<IParticipantStore>(provider => cosmosDb);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }

        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];
#pragma warning disable CA2000
            var cosmosClient = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
#pragma warning restore CA2000
            var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName).ConfigureAwait(false);
            _ = await database.Database.CreateContainerIfNotExistsAsync(containerName, "/PartitionKey").ConfigureAwait(false);
            var cosmosDbService = new CosmosDbService(cosmosClient, databaseName, containerName);
            return cosmosDbService;
        }
    }
}