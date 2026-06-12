using Microsoft.AspNetCore.Mvc;
using offboarding_prc_api.Services;
using offboarding_prc_api.Models;

namespace offboarding_prc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmsDataController : ControllerBase
    {
        private readonly TempCacheService _tempCacheService;

        public EmsDataController(TempCacheService tempCacheService)
        {
            _tempCacheService = tempCacheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeData()
        {
            // Extract token from the incoming request's Authorization header
            string authHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized(new { message = "Missing or invalid Authorization header." });

            string token = authHeader["Bearer ".Length..].Trim();

            var employees = await _tempCacheService.GetAllEmployeeInfoDirectAsync(token);
            return Ok(employees);
        }
    }
}