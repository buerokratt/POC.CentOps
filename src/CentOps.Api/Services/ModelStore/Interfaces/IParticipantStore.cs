using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services.ModelStore.Interfaces
{
    public interface IParticipantStore : IModelStore<ParticipantDto>
    {
        Task<IEnumerable<ParticipantDto>> GetAll(IEnumerable<ParticipantTypeDto> types, bool includeInactive = false);
        Task<ParticipantDto?> GetByApiKeyAsync(string apiKey);
    }
}
