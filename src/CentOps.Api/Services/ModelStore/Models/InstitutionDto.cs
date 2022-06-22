using CentOps.Api.Services.ModelStore.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Services.ModelStore.Models
{
    [ExcludeFromCodeCoverage]
    public class InstitutionDto : IModel
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public InstitutionStatusDto Status { get; set; }
    }
}
