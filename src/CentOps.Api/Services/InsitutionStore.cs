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

        Task<bool> IModelStore<Institution>.DeleteById(string id)
        {
            var model = _institutions.First(c => c.Id == id);

            if (model != null)
            {
                _ = _institutions.Remove(model);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        Task<IEnumerable<Institution>> IModelStore<Institution>.GetAll()
        {
            return Task.FromResult(_institutions.AsEnumerable());
        }

        Task<Institution?> IModelStore<Institution>.GetById(string id)
        {
            return Task.FromResult(_institutions.FirstOrDefault(x => x.Id == id));
        }

        Task<Institution> IModelStore<Institution>.Update(Institution model)
        {
            var idx = _institutions.FindIndex(x => x.Id == model.Id);
            _institutions[idx] = model;
            return Task.FromResult(model);
        }
    }
}
