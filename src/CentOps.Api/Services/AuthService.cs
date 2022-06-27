using CentOps.Api.Authentication.Models;

namespace CentOps.Api.Services
{
    public interface IAuthService
    {
        Task<ApiUser?> AuthenticateAsync(string apiKey);
    }

    public class AuthService : IAuthService
    {
        private readonly ApiUser[] _users = new ApiUser[]
        {
            new ApiUser() { ApiKey = "iamadmin", Id = "123", Name = "Ahmed", IsAdmin = true },
            new ApiUser() { ApiKey = "imnotadmin", Id = "234", Name = "NationalLibrary", IsAdmin = false }
        };

        public Task<ApiUser?> AuthenticateAsync(string apiKey)
        {
            var user = _users.Where(u => u.ApiKey == apiKey).SingleOrDefault();

            return Task.FromResult(user);
        }
    }
}
