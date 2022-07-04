
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CentOps.Api.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class CreateUpdateParticipantModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [Required]
        [JsonProperty(PropertyName = "institutionId")]
        public string? InstitutionId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        [JsonProperty(PropertyName = "host")]
        public string? Host { get; set; }

        [JsonProperty(PropertyName = "type")]
        public ParticipantType Type { get; set; }

        [JsonProperty(PropertyName = "status")]
        public ParticipantStatus Status { get; set; }
    }
}
