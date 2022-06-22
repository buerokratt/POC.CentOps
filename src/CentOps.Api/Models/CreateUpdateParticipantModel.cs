
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateUpdateParticipantModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Name { get; set; }

        [Required]
        public string? InstitutionId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string? Host { get; set; }

        public ParticipantType Type { get; set; }

        public ParticipantStatus Status { get; set; }
    }
}
