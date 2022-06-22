using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services
{
    public sealed class InstitutionStore : IInstitutionStore
    {
        private readonly List<InstitutionDto> _institutions = new();

        Task<InstitutionDto> IModelStore<InstitutionDto>.Create(InstitutionDto model)
        {
            model.Id = Guid.NewGuid().ToString();
            _institutions.Add(model);
            return Task.FromResult(model);
        }

        Task<bool> IModelStore<InstitutionDto>.DeleteById(string id)
        {
            var model = _institutions.First(c => c.Id == id);

            if (model != null)
            {
                _ = _institutions.Remove(model);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        Task<IEnumerable<InstitutionDto>> IModelStore<InstitutionDto>.GetAll()
        {
            return Task.FromResult(_institutions.AsEnumerable());
        }

        Task<InstitutionDto?> IModelStore<InstitutionDto>.GetById(string id)
        {
            return Task.FromResult(_institutions.FirstOrDefault(x => x.Id == id));
        }

        Task<InstitutionDto> IModelStore<InstitutionDto>.Update(InstitutionDto model)
        {
            var idx = _institutions.FindIndex(x => x.Id == model.Id);
            _institutions[idx] = model;
            return Task.FromResult(model);
        }
    }
}
