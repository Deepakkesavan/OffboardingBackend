using Offboarding_inner_api.Model;
using Offboarding_inner_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Offboarding_inner_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItDepartmentController : ControllerBase
    {
        private readonly IItDepartmentService _itDepartmentService;

        public ItDepartmentController(IItDepartmentService itDepartmentService)
        {
            _itDepartmentService = itDepartmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ItDepartment>>> GetAllItDepartments()
        {
            var departments = await _itDepartmentService.GetAllItDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("{itDepartmentId}")]
        public async Task<ActionResult<ItDepartment>> GetItDepartmentById(int itDepartmentId)
        {
            var department = await _itDepartmentService.GetItDepartmentByIdAsync(itDepartmentId);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult<ItDepartment>> AddItDepartment(ItDepartment itDepartment)
        {
            if (itDepartment == null)
            {
                return BadRequest();
            }

            var addedDepartment = await _itDepartmentService.AddItDepartmentAsync(itDepartment);

            return CreatedAtAction(
                nameof(GetItDepartmentById),
                new { itDepartmentId = addedDepartment.ItDepartmentId },
                addedDepartment
            );
        }

        [HttpPut("{itDepartmentId}")]
        public async Task<IActionResult> UpdateItDepartment(int itDepartmentId, ItDepartment itDepartment)
        {
            var isUpdated = await _itDepartmentService.UpdateItDepartmentAsync(itDepartmentId, itDepartment);

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{itDepartmentId}")]
        public async Task<IActionResult> DeleteItDepartment(int itDepartmentId)
        {
            var isDeleted = await _itDepartmentService.DeleteItDepartmentAsync(itDepartmentId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}