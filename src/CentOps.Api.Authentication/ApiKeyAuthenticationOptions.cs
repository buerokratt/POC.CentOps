using Microsoft.AspNetCore.Authentication;

namespace CentOps.Api.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ApiKeyHeaderName { get; set; } = ApiKeyAuthenciationDefaults.DefaultApiKeyHeaderName;
    }
}
