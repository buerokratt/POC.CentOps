using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.Api.Services
{
    public sealed partial class InMemoryStore : IParticipantStore, IInstitutionStore
    {
        public InMemoryStore()
        {
            _ = _apiUsers.TryAdd(DefaultAdminUser.Id ?? string.Empty, DefaultAdminUser);
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