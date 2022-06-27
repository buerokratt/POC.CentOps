using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Collections.Concurrent;

namespace CentOps.Api.Services
{
    public sealed class InMemoryStore : IParticipantStore, IInstitutionStore
    {
        private readonly ConcurrentDictionary<string, ParticipantDto> _participants = new();
        private readonly ConcurrentDictionary<string, InstitutionDto> _institutions = new();

        async Task<ParticipantDto> IModelStore<ParticipantDto>.Create(ParticipantDto model)
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

            _ = _participants.TryAdd(model.Id, model);

            return model;
        }

        Task<bool> IModelStore<ParticipantDto>.DeleteById(string id)
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

        Task<IEnumerable<ParticipantDto>> IModelStore<ParticipantDto>.GetAll()
        {
            return Task.FromResult(_participants.Values.AsEnumerable());
        }

        Task<ParticipantDto?> IModelStore<ParticipantDto>.GetById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? throw new ArgumentException($"{nameof(id)} not specified.")
                : _participants.ContainsKey(id)
                    ? Task.FromResult<ParticipantDto?>(_participants[id])
                    : Task.FromResult<ParticipantDto?>(null);
        }

        async Task<ParticipantDto> IModelStore<ParticipantDto>.Update(ParticipantDto model)
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
                throw new ArgumentException($"{nameof(id)} not specified.");
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

        private async Task CheckInstitution(string institutionId)
        {
            var institutionStore = this as IInstitutionStore;

            var institution = await institutionStore.GetById(institutionId).ConfigureAwait(false);
            if (institution == null)
            {
                throw new ModelNotFoundException<InstitutionDto>(institutionId);
            }
        }

        private async Task CheckAssociatedParticipantAsync(string institutionId)
        {
            var participantStore = this as IParticipantStore;

            var participants = await participantStore.GetAll().ConfigureAwait(false);
            var associatedParticipant = participants.FirstOrDefault(x => x.InstitutionId == institutionId);

            if (associatedParticipant != null)
            {
                throw new ModelExistsException<ParticipantDto>(associatedParticipant.Id!);
            }
        }
    }
}