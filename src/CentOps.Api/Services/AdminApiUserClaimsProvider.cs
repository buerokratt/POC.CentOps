using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Authentication.Models;
using CentOps.Api.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace CentOps.Api.Services
{
    public class AdminApiUserClaimsProvider : IApiUserClaimsProvider
    {
        private readonly AdminAuthConfig _config;

        public AdminApiUserClaimsProvider(AdminAuthConfig config)
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
                    new Claim("id", user.Id!),
                    new Claim("name", user.Name!),
                    new Claim("institutionId", user.InstitutionId!),
                    new Claim("status", user.Status.ToString())
                });
            }

            return Task.FromResult(user);
        }
    }
}
