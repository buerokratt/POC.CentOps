using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateUpdateInsitutionModel
    {
        public string? Name { get; set; }

        public InstitutionStatus Status { get; set; }
    }
}
