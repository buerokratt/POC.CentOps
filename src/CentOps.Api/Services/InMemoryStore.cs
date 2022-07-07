using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services
{
    public sealed partial class InMemoryStore : IParticipantStore, IInstitutionStore
    {
        public void SeedParticipants(params ParticipantDto[] seed)
        {
            if (seed == null)
            {
                return;
            }

            foreach (var participant in seed)
            {
                if (participant.Id != null)
                {
                    _ = _participants.TryAdd(participant.Id, participant);
                }
            }
        }

        public void SeedInstitutions(params InstitutionDto[] seed)
        {
            if (seed == null)
            {
                return;
            }

            foreach (var institution in seed)
            {
                if (institution.Id != null)
                {
                    _ = _institutions.TryAdd(institution.Id, institution);
                }
            }
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

        Task<IEnumerable<ParticipantDto>> IParticipantStore.GetAll(ParticipantTypeDto[] types)
        {
            return Task.FromResult(_participants.Values.Where(p => types.Contains(p.Type)).AsEnumerable());
        }
    }
}