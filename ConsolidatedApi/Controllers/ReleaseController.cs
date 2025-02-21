using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthApi.Interfaces;


namespace ConsolidatedAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class ReleaseController(IReleaseService releaseService) : ControllerBase
    {

        private readonly IReleaseService _releaseService = releaseService;

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> Consolidated()
        {
            return Ok(await _releaseService.Consolidated());
        }
    }
}
