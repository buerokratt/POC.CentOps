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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Institution))]
        public async Task<ActionResult<Institution>> Put()
        {
            string content;
            using (StreamReader reader = new(Request.Body, Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            var institution = JsonConvert.DeserializeObject<Institution>(content);
            return institution == null
                ? BadRequest()
                : (ActionResult<Institution>)Ok(await _store.Update(institution).ConfigureAwait(false));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<Institution>> Delete()
        {
            string id;
            using (StreamReader reader = new(Request.Body, Encoding.UTF8))
            {
                id = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            _ = await _store.DeleteById(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}
