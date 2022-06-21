using CentOps.Api.Models;
using CentOps.Api.Services;
using CentOps.Api.Services.Exceptions;
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

        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Participant>>> Get(string id)
        {
            var participant = await _store.GetById(id).ConfigureAwait(false);

            return participant != null
                ? Ok(participant)
                : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Participant>> Post(Participant participant)
        {
            try
            {
                var createdParticipant = await _store.Create(participant).ConfigureAwait(false);
                return Created(
                    new Uri($"/admin/participants/{createdParticipant.Name}", UriKind.Relative),
                    createdParticipant);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx);
            }
            catch (ModelNotFoundException<Institution> institutionEx)
            {
                return BadRequest(institutionEx.Message);
            }
            catch (ModelExistsException<Participant>)
            {
                return Conflict();
            }
        }

        [HttpPut("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Participant>> Put(Participant participant)
        {
            // Validate participant.
            try
            {
                var updatedParticipant = await _store.Update(participant).ConfigureAwait(false);
                return Ok(updatedParticipant);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx);
            }
            catch (ModelNotFoundException<Institution> institutionEx)
            {
                return BadRequest(institutionEx.Message);
            }
            catch (ModelNotFoundException<Participant> participantEx)
            {
                return BadRequest(participantEx.Message);
            }
        }

        [HttpDelete("name")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string name)
        {
            try
            {
                var deleted = await _store.DeleteById(name).ConfigureAwait(false);
                return deleted
                    ? NoContent()
                    : NotFound(name);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
    }
}
