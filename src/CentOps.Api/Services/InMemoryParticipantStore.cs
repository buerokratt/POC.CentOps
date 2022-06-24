using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Collections.Concurrent;

namespace CentOps.Api.Services
{
    public class InMemoryParticipantStore : IParticipantStore
    {
        private readonly ConcurrentDictionary<string, ParticipantDto> _participants = new();
        private readonly IInstitutionStore _institutionStore;

        public InMemoryParticipantStore(IInstitutionStore institutionStore)
        {
            _institutionStore = institutionStore;
        }

        public async Task<ParticipantDto> Create(ParticipantDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.InstitutionId))
            {
                throw new ArgumentException("Participant InstitutionId not specified.");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Participant Name not specified.");
            }

            await CheckInstitution(model.InstitutionId).ConfigureAwait(false);

            var existingName = _participants.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<ParticipantDto>(model.Name);
            }

            // Create an Id
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

        public Task<IEnumerable<ParticipantDto>> GetAll()
        {
            return Task.FromResult(_participants.Values.AsEnumerable());
        }

        public Task<ParticipantDto?> GetById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : _participants.ContainsKey(id)
                    ? Task.FromResult<ParticipantDto?>(_participants[id])
                    : Task.FromResult<ParticipantDto?>(null);
        }

        public async Task<ParticipantDto> Update(ParticipantDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                throw new ArgumentException($"Participant Id not specified.");
            }

            if (string.IsNullOrEmpty(model.InstitutionId))
            {
                throw new ArgumentException("Participant InstitutionId not specified.");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Participant Name not specified.");
            }

            await CheckInstitution(model.InstitutionId).ConfigureAwait(false);

            if (!_participants.ContainsKey(model.Id))
            {
                throw new ModelNotFoundException<ParticipantDto>(model.Id);
            }

            _participants[model.Id] = model;

            return model;
        }

        private async Task CheckInstitution(string institutionId)
        {
            var institution = await _institutionStore.GetById(institutionId).ConfigureAwait(false);
            if (institution == null)
            {
                throw new ModelNotFoundException<InstitutionDto>(institutionId);
            }
        }
    }
}
