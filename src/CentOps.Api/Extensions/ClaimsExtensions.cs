namespace CentOps.Api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetApiUserId(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var id = httpContext.User.Claims.First(c => c.Type == "id").Value;

            return id;
        }
    }
}
