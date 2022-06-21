using CentOps.Api.Models;

namespace CentOps.Api.Services
{
    public sealed class InstitutionStore : IInstitutionStore
    {
        private readonly List<Institution> _institutions = new();

         Task<Institution> IModelStore<Institution>.Create(Institution model)
        {
            _institutions.Add(model);
            return Task.FromResult(model);
        }

        Task<Institution> IModelStore<Institution>.DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Institution>> IModelStore<Institution>.GetAll()
        {
            return Task.FromResult(_institutions.AsEnumerable());
        }

        Task<Institution> IModelStore<Institution>.GetById(string id)
        {
            return Task.FromResult(_institutions.First(x => x.Id == id));
        }

        Task<Institution> IModelStore<Institution>.Update(Institution model)
        {
            throw new NotImplementedException();
        }
    }
}
