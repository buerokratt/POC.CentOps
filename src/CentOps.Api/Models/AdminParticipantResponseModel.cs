using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Models
{
    // Excluded as model contains no logic.
    [ExcludeFromCodeCoverage]
    public class AdminParticipantResponseModel : ParticipantResponseModel
    {
        public string? ApiKey { get; set; }
    }
}
