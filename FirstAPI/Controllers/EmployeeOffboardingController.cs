using FirstAPI.Model;
using FirstAPI.Services;
using FirstAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeOffboardingController : ControllerBase
    {
        private readonly IEmployeeOffboardingService _employeeOffboardingService;
        public EmployeeOffboardingController(IEmployeeOffboardingService employeeOffboardingService)
        {
            _employeeOffboardingService = employeeOffboardingService;
        }
        [HttpGet]
        public async Task<ActionResult<List<EmployeeOffboard>>> GetAllEmployeeOffboardings()
        {
            var employees = await _employeeOffboardingService.GetAllEmployeeOffboardingsAsync();
            return Ok(employees);
        }
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeOffboard>> GetEmployeeOffboardingsById(int employeeId)
        {
            var employeeOffboard = await _employeeOffboardingService.GetEmployeeOffboardingByIdAsync(employeeId);
            if (employeeOffboard == null)
            {
                return NotFound();
            }
            return Ok(employeeOffboard);
        }
        [HttpPost]
        public async Task<ActionResult<EmployeeOffboard>> AddEmployeeOffboardings(EmployeeOffboard employeeOffboard)
        {
            if (employeeOffboard == null)
            {
                return BadRequest();
            }
            var addedEmployee = await _employeeOffboardingService.AddEmployeeOffboardingAsync(employeeOffboard);
            return CreatedAtAction(
                nameof(GetEmployeeOffboardingsById),
                new { employeeId = addedEmployee.EmployeeId },
                addedEmployee
            );
        }
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeOffboardings(int employeeId, EmployeeOffboard employeeOffboard)
        {
            var isUpdated = await _employeeOffboardingService.UpdateEmployeeOffboardingAsync(employeeId, employeeOffboard);
            if (!isUpdated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployeeOffboardings(int employeeId)
        {
            var isDeleted = await _employeeOffboardingService.DeleteEmployeeOffboardingAsync(employeeId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}