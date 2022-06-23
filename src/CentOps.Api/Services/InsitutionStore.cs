using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Collections.Concurrent;

namespace CentOps.Api.Services
{
    public sealed class InstitutionStore : IInstitutionStore
    {
        private readonly ConcurrentDictionary<string, InstitutionDto> _institutions = new();

        public Task<InstitutionDto> Create(InstitutionDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Institution Name not specified.");
            }
            model.Id = Guid.NewGuid().ToString();
            _ = _institutions.TryAdd(model.Id, model);
            return Task.FromResult(model);
        }
        public Task<bool> DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"{nameof(id)} not specified.");
            }

            //Dont delete Institution that has associated Participants 
            //go thru all the Participants and see if they have same Institution id
            if (_institutions.ContainsKey(id))
            {
                _ = _institutions.Remove(id, out _);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<IEnumerable<InstitutionDto>> GetAll()
        {
            return Task.FromResult(_institutions.Values.AsEnumerable());
        }

        public Task<InstitutionDto?> GetById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : _institutions.ContainsKey(id)
                    ? Task.FromResult<InstitutionDto?>(_institutions[id])
                    : Task.FromResult<InstitutionDto?>(null);
        }

        public Task<InstitutionDto> Update(InstitutionDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
            {
                throw new ArgumentException($"Institution or Institution Id not specified.");
            }

            if (!_institutions.ContainsKey(model.Id))
            {
                throw new ModelNotFoundException<InstitutionDto>(model.Id);
            }

            _institutions[model.Id] = model;

            return Task.FromResult(model);
        }

    }
}
