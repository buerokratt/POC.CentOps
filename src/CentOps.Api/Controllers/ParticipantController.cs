using CentOps.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("admin/participants")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantStore _store;

        public ParticipantController(IParticipantStore store)
        {
            _store = store;
        }
    }
}
