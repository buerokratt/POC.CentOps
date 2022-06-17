using CentOps.Api.Services;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    [ExcludeFromCodeCoverage]
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
