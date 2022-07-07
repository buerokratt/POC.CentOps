namespace CentOps.Api.Configuration
{
    public class AuthConfig
    {
        internal const string AdminPolicy = "AdminPolicy";
        internal const string ParticipantPolicy = "ParticipantPolicy";

        public string AdminApiKey { get; set; } = string.Empty;
    }
}
