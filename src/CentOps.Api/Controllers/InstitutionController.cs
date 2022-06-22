using AutoMapper;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("admin/institutions")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionStore _store;
        private readonly IMapper _mapper;

        public InstitutionController(IInstitutionStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionResponseModel>>> Get()
        {
            return Ok(await _store.GetAll().ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<InstitutionResponseModel>> Get(string name)
        {
            return Ok(await _store.GetById(name).ConfigureAwait(false));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(InstitutionResponseModel))]
        public async Task<ActionResult<InstitutionResponseModel>> Post(CreateUpdateInsitutionModel institution)
        {
            var institutionDTO = _mapper.Map<InstitutionDto>(institution);

            var createdInsitution = await _store.Create(institutionDTO).ConfigureAwait(false);
            return institution == null ? BadRequest() : Ok(_mapper.Map<InstitutionResponseModel>(createdInsitution));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateUpdateInsitutionModel))]
        public async Task<ActionResult<InstitutionResponseModel>> Put(string id, CreateUpdateInsitutionModel institution)
        {
            var institutionDTO = _mapper.Map<InstitutionDto>(institution);
            institutionDTO.Id = id;

            var response = await _store.Update(institutionDTO).ConfigureAwait(false);
            return Ok(_mapper.Map<InstitutionResponseModel>(response));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(string id)
        {
            _ = await _store.DeleteById(id).ConfigureAwait(false);


            return NoContent();
        }
    }
}
