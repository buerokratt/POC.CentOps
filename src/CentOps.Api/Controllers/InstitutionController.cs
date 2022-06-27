using CentOps.Api.Models;
using CentOps.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("admin/institutions")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly IInsitutionStore _store;

        public InstitutionController(IInsitutionStore store)
        {
            _store = store;
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<IEnumerable<Institution>>> Get()
        {
            return Ok(await _store.GetAll().ConfigureAwait(false));
        }
    }
}
