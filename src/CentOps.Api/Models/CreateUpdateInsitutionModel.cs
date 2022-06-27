using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class CreateUpdateInsitutionModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Name { get; set; }

        public InstitutionStatus Status { get; set; }
    }
}
