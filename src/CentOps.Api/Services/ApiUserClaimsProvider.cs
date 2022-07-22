using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Authentication.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using System.Security.Claims;

namespace CentOps.Api.Services
{
    public class ApiUserClaimsProvider : IApiUserClaimsProvider
    {
        private readonly IParticipantStore _participantStore;

        public ApiUserClaimsProvider(IParticipantStore participantStore)
        {
            _participantStore = participantStore;
        }

        public async Task<ApiUser?> GetUserClaimsAsync(string apiKey)
        {
            var user = await _participantStore.GetByApiKeyAsync(apiKey).ConfigureAwait(false);

            ApiUser? result = null;

            if (user != null && user.Status != ParticipantStatusDto.Deleted)
            {
                result = new ApiUser(new[]
                {
                    new Claim("id", user.Id!),
                    new Claim("pk", user.PartitionKey!),
                    new Claim("name", user.Name!),
                    new Claim("institutionId", user.InstitutionId!),
                    new Claim("status", user.Status.ToString())
                });
            }

            return result;
        }
    }
}
