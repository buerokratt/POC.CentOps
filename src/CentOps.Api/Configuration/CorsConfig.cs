using System.Collections.ObjectModel;

namespace CentOps.Api.Configuration
{
    public class CorsConfig
    {
        public Collection<string> AllowedOrigins { get; private set; } = new Collection<string>();
    }
}
