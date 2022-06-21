using CentOps.Api.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class Participant : IModel
    {
        public string? Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Name { get; set; }

        [Required]
        public string? InstitutionName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Host { get; set; }

        public ParticipantType Type { get; set; }

        public ParticipantStatus Status { get; set; }
    }
}
