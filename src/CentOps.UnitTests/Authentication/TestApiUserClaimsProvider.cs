using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Authentication.Models;

namespace CentOps.UnitTests.Authentication
{
    public class TestApiUserClaimsProvider : IApiUserClaimsProvider
    {
        public IDictionary<string, ApiUser> Users { get; private set; }

        public void Setup(IDictionary<string, ApiUser> users)
        {
            Users = users;
        }

        public Task<ApiUser> GetUserClaimsAsync(string apiKey)
        {
            var user = Users.ContainsKey(apiKey) ? Users[apiKey] : null;

            return Task.FromResult(user);
        }
    }
}
