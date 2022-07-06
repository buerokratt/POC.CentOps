using CentOps.Api.Authentication.Models;

namespace CentOps.Api.Authentication.Interfaces
{
    /// <summary>
    /// An interface that provides consumers an abstraction to authenticate API Keys within their own systems.
    /// </summary>
    public interface IApiUserClaimsProvider
    {
        /// <summary>
        /// Gets the 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        Task<ApiUser?> GetUserClaimsAsync(string apiKey);
    }
}
