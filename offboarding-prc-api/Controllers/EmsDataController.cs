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
            var employees = await _tempCacheService.GetAllEmployeeInfoAsync();

            return Ok(employees);
        }
    }
}