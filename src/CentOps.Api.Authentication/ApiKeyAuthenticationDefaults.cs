using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api.Authentication
{
    [ExcludeFromCodeCoverage] // No logic in this file
    public static class ApiKeyAuthenticationDefaults
    {
        public static readonly string AdminAuthenticationScheme = "AdminApiKey";
        public static readonly string AuthenticationScheme = "ApiKey";
        public static readonly string DefaultApiKeyHeaderName = "X-Api-Key";
    }
}
