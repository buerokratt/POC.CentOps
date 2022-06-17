using CentOps.Api.Models;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Participant>>> Get()
        {
            return Ok(await _store.GetAll().ConfigureAwait(false));
        }
    }
}
