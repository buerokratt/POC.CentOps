namespace CentOps.Api.Configuration
{
    public class CorsConfig
    {
        public IEnumerable<string> AllowedOrigins { get; set; } = Array.Empty<string>();
    }
}
