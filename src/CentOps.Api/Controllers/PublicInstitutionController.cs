using AutoMapper;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("public/institutions")]
    [ApiController]
    public class PublicInstitutionController : ControllerBase
    {
        private readonly IInstitutionStore _store;
        private readonly IMapper _mapper;

        public PublicInstitutionController(IInstitutionStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionResponseModel>>> Get()
        {
            var institutions = await _store.GetAll().ConfigureAwait(false);
            return Ok(_mapper.Map<IEnumerable<InstitutionResponseModel>>(institutions));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ParticipantResponseModel>>> Get(string id)
        {
            var instituion = await _store.GetById(id).ConfigureAwait(false);

            if (instituion == null)
            {
                return NotFound(id);
            }

            var participants = await _store.GetParticipantsByInstitutionId(id).ConfigureAwait(false);

            return Ok(_mapper.Map<IEnumerable<ParticipantResponseModel>>(participants));
        }
    }
}
