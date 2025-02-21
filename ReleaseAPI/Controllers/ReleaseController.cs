using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthApi.Interfaces;
using ReleaseAPI.Dtos;


namespace ReleaseAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class ReleaseController(IReleaseService releaseService) : ControllerBase
    {
        private readonly IReleaseService _releaseService = releaseService;

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] ReleaseRequest request)
        {
            return Ok(await _releaseService.Create(request));
        }
    }
}
