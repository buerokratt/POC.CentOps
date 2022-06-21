using CentOps.Api.Models;
using CentOps.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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
        public async Task<ActionResult<Institution>> Get(string id)
        {
            return Ok(await _store.GetById(id).ConfigureAwait(false));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Institution))]
        public async Task<ActionResult<Institution>> Post()
        {
            string content;
            using (StreamReader reader = new(Request.Body, Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            var institution = JsonConvert.DeserializeObject<Institution>(content);
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
