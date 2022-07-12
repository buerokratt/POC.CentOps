using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("/version")]
    [AllowAnonymous]
    public class VersionController : ControllerBase
    {
        private readonly string _version;

        public VersionController(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(argument: configuration);

            _version = configuration
                .GetSection("Settings")
                .GetValue<string>("Version");
        }

        [HttpGet]
        public Task<string> GetVersion()
        {
            return Task.FromResult(_version);
        }
    }
}
