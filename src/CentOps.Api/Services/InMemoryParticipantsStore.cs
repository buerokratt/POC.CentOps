using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Collections.Concurrent;

namespace CentOps.Api.Services
{
    public sealed partial class InMemoryStore : IParticipantStore
    {
        private readonly ConcurrentDictionary<string, ParticipantDto> _participants = new();

        public async Task<ParticipantDto> Create(ParticipantDto model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.InstitutionId))
            {
                throw new ArgumentException(nameof(model.InstitutionId));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            await CheckInstitution(model.InstitutionId).ConfigureAwait(false);

            var existingName = _participants.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<ParticipantDto>(model.Name);
            }

            // Create an Id
            model.Id = Guid.NewGuid().ToString();
            model.PartitionKey = $"participant::{model.Id}";

            _ = _participants.TryAdd(model.Id, model);

            return model;
        }

        public Task<bool> DeleteById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
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
                ? throw new ArgumentNullException(nameof(id))
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
                throw new ArgumentException(nameof(model.Id));
            }

            if (string.IsNullOrEmpty(model.InstitutionId))
            {
                throw new ArgumentException(nameof(model.InstitutionId));
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException(nameof(model.Name));
            }

            await CheckInstitution(model.InstitutionId).ConfigureAwait(false);

            var existingName = _participants.Values.FirstOrDefault(i => i.Name == model.Name);
            if (existingName != null)
            {
                throw new ModelExistsException<ParticipantDto>(model.Name!);
            }

            if (!_participants.ContainsKey(model.Id))
            {
                throw new ModelNotFoundException<ParticipantDto>(model.Id);
            }

            _participants[model.Id] = model;

            return model;
        }

        Task<ParticipantDto?> IParticipantStore.GetByApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var participant = _participants.Values.FirstOrDefault(p => p.ApiKey == apiKey);
            return Task.FromResult(participant);
        }

        Task<IEnumerable<ParticipantDto>> IParticipantStore.GetAll(IEnumerable<ParticipantTypeDto> types, bool includeInactive)
        {
            var queryable = _participants.Values.AsEnumerable();

            if (types != null && types.Any())
            {
                queryable = queryable.Where(p => types.Contains(p.Type));
            }

            if (includeInactive == false)
            {
                queryable = queryable.Where(p => p.Status == ParticipantStatusDto.Active);
            }

            return Task.FromResult(queryable.AsEnumerable());
        }

        public Task<ParticipantDto> UpdateState(string id, string partitionKey, ParticipantStatusDto newStatus)
        {
            ArgumentNullException.ThrowIfNull(id);

            if (newStatus is not ParticipantStatusDto.Active and not ParticipantStatusDto.Disabled)
            {
                throw new ArgumentException($"Invalid new state value: {newStatus}");
            }

            if (!_participants.TryGetValue(id, out var participant))
            {
                throw new ModelNotFoundException<ParticipantDto>(id);
            }

            participant.Status = newStatus;

            return Task.FromResult(participant);

        }
    }
}