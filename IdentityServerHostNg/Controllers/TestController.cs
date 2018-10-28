using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Host.Ng.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [Authorize(AuthenticationSchemes = "Bearer", Policy = Config.AuthenticatedPolicy)]
        [HttpGet("publicapi", Name = nameof(PublicApi))]
        public IActionResult PublicApi()
        {
            return Ok("PUBLIC API REPLY");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = Config.AdministrationApiPolicy)]
        [HttpGet("administrationapi", Name = nameof(AdministrationApi))]
        public IActionResult AdministrationApi()
        {
            return Ok("ADMINISTRATION API REPLY");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = Config.AdvancedApiPolicy)]
        [HttpGet("advancedapi", Name = nameof(AdvancedApi))]
        public IActionResult AdvancedApi()
        {
            return Ok("ADVANCED API REPLY");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = Config.BasicApiPolicy)]
        [HttpGet("basicapi", Name = nameof(BasicApi))]
        public IActionResult BasicApi()
        {
            return Ok("BASIC API REPLY");
        }
    }
}
