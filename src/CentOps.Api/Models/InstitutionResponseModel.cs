using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class InstitutionResponseModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public InstitutionStatus Status { get; set; }
    }
}
