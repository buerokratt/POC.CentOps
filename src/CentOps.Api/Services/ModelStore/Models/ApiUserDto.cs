using CentOps.Api.Services.ModelStore.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Services.ModelStore.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class ApiUserDto : IModel
    {
        public string? Id { get; set; } = "";
        public string? Name { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public bool IsAdmin { get; set; }
    }
}
