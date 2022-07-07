using System.Security.Claims;

namespace CentOps.Api.Authentication.Models
{
    public class ApiUser
    {
        public ICollection<Claim> Claims { get; }

        public ApiUser(ICollection<Claim> claims)
        {
            Claims = claims;
        }
    }
}
