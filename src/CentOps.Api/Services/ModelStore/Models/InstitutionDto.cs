using CentOps.Api.Services.ModelStore.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Services.ModelStore.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class InstitutionDto : IModel
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "pk")]
        public string? PartitionKey { get; set; }

        [JsonProperty(PropertyName = "status")]
        public InstitutionStatusDto Status { get; set; }
    }
}
