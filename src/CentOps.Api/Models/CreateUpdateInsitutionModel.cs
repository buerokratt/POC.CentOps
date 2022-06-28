using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CentOps.Api.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class CreateUpdateInsitutionModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "status")]
        public InstitutionStatus Status { get; set; }
    }
}
