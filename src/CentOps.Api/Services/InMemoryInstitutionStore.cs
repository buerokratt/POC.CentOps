using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Collections.Concurrent;

namespace CentOps.Api.Services
{
    public sealed partial class InMemoryStore : IInstitutionStore
    {
        private readonly ConcurrentDictionary<string, InstitutionDto> _institutions = new();

        Task<InstitutionDto> IModelStore<InstitutionDto>.Create(InstitutionDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            var existingName = _institutions.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<InstitutionDto>(model.Name);
            }

            model.Id = Guid.NewGuid().ToString();
            _ = _institutions.TryAdd(model.Id, model);
            return Task.FromResult(model);
        }

        async Task<bool> IModelStore<InstitutionDto>.DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), $"{nameof(id)} not specified.");
            }

            if (_institutions.ContainsKey(id))
            {
                await CheckAssociatedParticipantAsync(id).ConfigureAwait(false);
                _ = _institutions.Remove(id, out _);
                return true;
            }

            return false;
        }

        Task<IEnumerable<InstitutionDto>> IModelStore<InstitutionDto>.GetAll()
        {
            return Task.FromResult(_institutions.Values.AsEnumerable());
        }

        Task<InstitutionDto?> IModelStore<InstitutionDto>.GetById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : _institutions.ContainsKey(id)
                    ? Task.FromResult<InstitutionDto?>(_institutions[id])
                    : Task.FromResult<InstitutionDto?>(null);
        }

        Task<InstitutionDto> IModelStore<InstitutionDto>.Update(InstitutionDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                throw new ArgumentException(nameof(model.Id));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            if (!_institutions.ContainsKey(model.Id))
            {
                throw new ModelNotFoundException<InstitutionDto>(model.Id);
            }

            var existingName = _institutions.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<InstitutionDto>(model.Name!);
            }

            _institutions[model.Id] = model;

            return Task.FromResult(model);
        }

        Task<InstitutionDto?> IModelStore<InstitutionDto>.GetByApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var institution = _institutions.Values.FirstOrDefault(i => i.ApiKey == apiKey);
            return Task.FromResult(institution);
        }

        public Task<IEnumerable<ParticipantDto>> GetParticipantsByInstitutionId(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : Task.FromResult(_participants.Values.Where(key => id.Equals(key.InstitutionId, StringComparison.Ordinal)).AsEnumerable());
        }
    }
}
