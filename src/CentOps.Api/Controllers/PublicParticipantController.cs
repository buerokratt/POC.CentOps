using AutoMapper;
using CentOps.Api.Configuration;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("public/participants")]
    [ApiController]
    [Authorize(Policy = AuthConfig.ParticipantPolicy)]
    public class PublicParticipantController : ControllerBase
    {
        private readonly IParticipantStore _store;
        private readonly IMapper _mapper;

        public PublicParticipantController(IParticipantStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ParticipantResponseModel>>> Get()
        {
            var participants = await _store.GetAll().ConfigureAwait(false);

            return Ok(_mapper.Map<IEnumerable<ParticipantResponseModel>>(participants));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParticipantResponseModel>> Get(string id)
        {
            var participant = await _store.GetById(id).ConfigureAwait(false);

            return participant != null
                ? Ok(_mapper.Map<ParticipantResponseModel>(participant))
                : NotFound(id);
        }
    }
}
