using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("/version")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public Task<string> GetVersion()
        {
            return Task.FromResult("1.2.3");
        }
    }
}
