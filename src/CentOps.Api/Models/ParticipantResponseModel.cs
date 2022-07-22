using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class ParticipantResponseModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? InstitutionId { get; set; }

        public string? Host { get; set; }

        public ParticipantType Type { get; set; }

        public ParticipantStatus Status { get; set; }

        public ParticipantState State { get; set; }
    }
}
