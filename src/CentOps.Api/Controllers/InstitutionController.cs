using AutoMapper;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Exceptions;
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
            var institutions = await _store.GetAll().ConfigureAwait(false);
            return Ok(_mapper.Map<IEnumerable<InstitutionResponseModel>>(institutions));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InstitutionResponseModel>> Get(string id)
        {
            var institution = await _store.GetById(id).ConfigureAwait(false);

            return institution != null
                ? Ok(_mapper.Map<InstitutionResponseModel>(institution))
                : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(InstitutionResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<InstitutionResponseModel>> Post(CreateUpdateInsitutionModel institution)
        {
            try
            {
                var institutionDTO = _mapper.Map<InstitutionDto>(institution);
                var createdInstitution = await _store.Create(institutionDTO).ConfigureAwait(false);
                return Created(
                    new Uri($"/admin/institutions/{createdInstitution.Name}", UriKind.Relative),
                    _mapper.Map<InstitutionResponseModel>(createdInstitution));
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx);
            }
            catch (ModelExistsException<InstitutionDto>)
            {
                return Conflict();
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateUpdateInsitutionModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InstitutionResponseModel>> Put(string id, CreateUpdateInsitutionModel institution)
        {
            try
            {
                var institutionDTO = _mapper.Map<InstitutionDto>(institution);
                institutionDTO.Id = id;
                var updatedInstitution = await _store.Update(institutionDTO).ConfigureAwait(false);
                return Ok(_mapper.Map<InstitutionResponseModel>(updatedInstitution));
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx);
            }
            catch (ModelNotFoundException<InstitutionDto> institutionEx)
            {
                return BadRequest(institutionEx.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _store.DeleteById(id).ConfigureAwait(false);
                return deleted
                    ? NoContent()
                    : NotFound(id);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
        }
    }
}
