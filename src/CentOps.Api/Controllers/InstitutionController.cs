using CentOps.Api.Services;
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
    }
}
