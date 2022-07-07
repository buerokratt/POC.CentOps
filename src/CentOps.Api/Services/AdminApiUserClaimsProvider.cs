using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Authentication.Models;
using CentOps.Api.Configuration;
using System.Security.Claims;

namespace CentOps.Api.Services
{
    public class AdminApiUserClaimsProvider : IApiUserClaimsProvider
    {
        private readonly AuthConfig _config;

        public AdminApiUserClaimsProvider(AuthConfig config)
        {
            _config = config;
        }

        public Task<ApiUser?> GetUserClaimsAsync(string apiKey)
        {
            ApiUser? user = null;
            if (_config.AdminApiKey == apiKey)
            {
                user = new ApiUser(new[]
                {
                    new Claim("admin", bool.TrueString)
                });
            }

            return Task.FromResult(user);
        }
    }
}
