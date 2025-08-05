using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [Authorize]
        [HttpGet("home")]
        public async Task<IActionResult> GetHomeData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token");

            var result = await _homeService.GetHomeData(userId);

            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }
    }
}
