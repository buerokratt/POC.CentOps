using CentOps.Api.Services.ModelStore.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Services.ModelStore.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class ParticipantDto : IModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? PartitionKey { get; set; }

        public string? InstitutionId { get; set; }

        public string? Host { get; set; }

        public ParticipantTypeDto Type { get; set; }

        public ParticipantStatusDto Status { get; set; }

        public string? ApiKey { get; set; }
    }
}
