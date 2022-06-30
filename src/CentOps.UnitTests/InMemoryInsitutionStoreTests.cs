using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.UnitTests
{
    public class InMemoryInsitutionStoreTests : BaseInstitutionStoreTests
    {
        private readonly InMemoryStore store = new();

        protected override IInstitutionStore GetInstitutionStore(params InstitutionDto[] seedInstitutions)
        {
            store.SeedInstitutions(seedInstitutions);

            return store;
        }

        protected override IParticipantStore GetParticipantStore(params ParticipantDto[] seedParticipants)
        {
            store.SeedParticipants(seedParticipants);

            return store;
        }
    }
}
