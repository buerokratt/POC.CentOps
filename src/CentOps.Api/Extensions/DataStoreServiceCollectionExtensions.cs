using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;

namespace CentOps.Api.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void AddDataStores(this IServiceCollection services)
        {
            var inMemoryStore = new InMemoryStore();
            _ = services.AddSingleton<IInstitutionStore>(provider => inMemoryStore);
            _ = services.AddSingleton<IParticipantStore>(provider => inMemoryStore);
        }
    }
}
