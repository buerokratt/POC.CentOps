using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.UnitTests.Services
{
    public class InMemoryInsitutionStoreTests : BaseInstitutionStoreTests
    {
        private readonly InMemoryStore store = new();

        protected override IInstitutionStore GetInstitutionStore(params InstitutionDto[] seed)
        {
            store.SeedInstitutions(seed);

            return store;
        }

        protected override IParticipantStore GetParticipantStore(params ParticipantDto[] seed)
        {
            store.SeedParticipants(seed);

            return store;
        }
    }
}
