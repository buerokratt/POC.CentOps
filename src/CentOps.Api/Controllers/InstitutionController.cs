using CentOps.Api.Models;
using CentOps.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("admin/institutions")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionStore _store;

        public InstitutionController(IInstitutionStore store)
        {
            _store = store;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Institution>>> Get()
        {
            return Ok(await _store.GetAll().ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Institution>> Get(string name)
        {
            return Ok(await _store.GetById(name).ConfigureAwait(false));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Institution))]
        public async Task<ActionResult<Institution>> Post(Institution institution)
        {
            return institution == null ? BadRequest() : await _store.Create(institution).ConfigureAwait(false);
        }

        [HttpPut]
        public async Task<ActionResult<Institution>> Put()
        {
            return Ok(await _store.GetAll().ConfigureAwait(false));
        }

        [HttpDelete]
        public async Task<ActionResult<Institution>> Delete()
        {
            return Ok(await _store.GetAll().ConfigureAwait(false));
        }
    }
}
