using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.UnitTests
{
    public class InMemoryInsitutionStoreTests : BaseInstitutionStoreTests
    {
        protected override IInstitutionStore GetStore(params InstitutionDto[] seedInstitutions)
        {
            return new InMemoryStore();
        }
    }
}
