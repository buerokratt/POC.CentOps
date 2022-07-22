using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Services.ModelStore.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class ParticipantDto : IModel
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "pk")]
        public string? PartitionKey { get; set; }

        [JsonProperty(PropertyName = "institutionId")]
        public string? InstitutionId { get; set; }

        [JsonProperty(PropertyName = "host")]
        public string? Host { get; set; }

        [JsonProperty(PropertyName = "type")]
        public ParticipantTypeDto Type { get; set; }

        [JsonProperty(PropertyName = "status")]
        public ParticipantStatusDto Status { get; set; }

        [JsonProperty(PropertyName = "apiKey")]
        public string? ApiKey { get; set; }
    }
}
