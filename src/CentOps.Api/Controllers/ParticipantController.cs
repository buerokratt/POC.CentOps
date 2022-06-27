using CentOps.Api.Models;
using CentOps.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("admin/participants")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
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

        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[Authorize(Policy = "UserPolicy")]
        public Task<IActionResult> Post(string id)
        {
            return Task.FromResult<IActionResult>(Ok(id));
        }
    }
}
