using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services.ModelStore.Interfaces
{
    public interface IInstitutionStore : IModelStore<InstitutionDto>
    {
        Task<IEnumerable<ParticipantDto>> GetParticipantsByInstitutionId(string id);
    }
}
