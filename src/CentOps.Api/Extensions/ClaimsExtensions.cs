namespace CentOps.Api.Extensions
{
    public static class ClaimsExtensions
    {
        public static (string id, string partitionKey) GetApiUser(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var id = httpContext.User.Claims.First(c => c.Type == "id").Value;
            var partitionKey = httpContext.User.Claims.First(c => c.Type == "pk").Value;

            return (id, partitionKey);
        }
    }
}
