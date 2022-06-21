using CentOps.Api.Models;
using CentOps.Api.Services.Exceptions;
using System.Collections.Concurrent;
using System.Linq;

namespace CentOps.Api.Services
{
    public class InMemoryParticipantStore : IParticipantStore
    {
        private readonly ConcurrentDictionary<string, Participant> _participants = new();
        private readonly IInstitutionStore _institutionStore;

        public InMemoryParticipantStore(IInstitutionStore institutionStore)
        {
            _institutionStore = institutionStore;
        }

        public async Task<Participant> Create(Participant model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.InstitutionName) ||
                string.IsNullOrEmpty(model.Id) ||
                string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Institution not specified.");
            }

            await CheckInstitution(model.InstitutionName).ConfigureAwait(false);

            var existingName = _participants.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<Participant>(model.Id);
            }

            model.Id = Guid.NewGuid().ToString();
            _ = _participants.TryAdd(model.Id, model);

            return model;
        }

        public Task<bool> DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"{nameof(id)} not specified.");
            }

            if (_participants.ContainsKey(id))
            {
                _ = _participants.Remove(id, out _);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<IEnumerable<Participant>> GetAll()
        {
            return Task.FromResult(_participants.Values.AsEnumerable());
        }

        public Task<Participant?> GetById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : _participants.ContainsKey(id)
                    ? Task.FromResult<Participant?>(_participants[id])
                    : Task.FromResult<Participant?>(null);
        }

        public async Task<Participant> Update(Participant model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
            {
                throw new ArgumentException($"Model or model.id not specified.");
            }

            if (string.IsNullOrEmpty(model.InstitutionName) || string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Institution not specified.");
            }

            await CheckInstitution(model.InstitutionName).ConfigureAwait(false);

            if (!_participants.ContainsKey(model.Name))
            {
                throw new ModelNotFoundException<Participant>(model.Name);
            }

            _participants[model.Name] = model;

            return model;
        }

        private async Task CheckInstitution(string institutionId)
        {
            var institution = await _institutionStore.GetById(institutionId).ConfigureAwait(false);
            if (institution == null)
            {
                throw new ModelNotFoundException<Institution>(institutionId);
            }
        }
    }
}
