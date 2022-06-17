using CentOps.Api.Services;

namespace CentOps.Api.Models
{
    public class Participant : IModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? InstitutionId { get; set; }

        public string? Host { get; set; }

        public ParticipantType Type { get; set; }

        public ParticipantStatus Status { get; set; }
    }
}
