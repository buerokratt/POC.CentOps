using CentOps.Api.Services.ModelStore.Models;

namespace CentOps.UnitTests
{
    internal static class DtoBuilder
    {
        public static InstitutionDto GetInstitution(
            string id = "1234",
            string name = "DefaultInstitution",
            InstitutionStatusDto status = InstitutionStatusDto.Active)
        {
            return new InstitutionDto
            {
                Id = id,
                PartitionKey = $"institution::{id}",
                Name = name,
                Status = status
            };
        }

        public static ParticipantDto GetParticipant(
            string id = "123",
            string name = "Test",
            string host = "https://host:8080",
            string institutionId = "1234",
            ParticipantStatusDto status = ParticipantStatusDto.Active,
            ParticipantTypeDto type = ParticipantTypeDto.Chatbot)
        {
            var institution = GetInstitution(id: institutionId, name: $"institution{institutionId}");
            return new ParticipantDto
            {
                Id = id,
                PartitionKey = $"participant::{id}",
                Name = name,
                Host = host,
                Status = status,
                Type = type,
                InstitutionId = institution.Id,
                ApiKey = $"supersecret_{id}"
            };
        }
    }
}
