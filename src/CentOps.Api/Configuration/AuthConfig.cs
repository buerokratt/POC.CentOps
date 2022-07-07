using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Configuration
{
    [ExcludeFromCodeCoverage] // Plain config object. Doesn't need tests.
    public class AuthConfig
    {
        internal const string AdminPolicy = "AdminPolicy";
        internal const string ParticipantPolicy = "ParticipantPolicy";

        public string AdminApiKey { get; set; } = string.Empty;
    }
}
