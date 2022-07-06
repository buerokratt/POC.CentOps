using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services.ModelStore.Interfaces
{
    public interface IParticipantStore : IModelStore<ParticipantDto>
    {
        Task<ParticipantDto?> GetByApiKeyAsync(string apiKey);
    }
}
