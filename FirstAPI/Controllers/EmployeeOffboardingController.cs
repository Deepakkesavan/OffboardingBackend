using System.Reflection.Metadata;
using FirstAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeOffboardingController : ControllerBase
    {
        private EmployeeDetailsContext _empDetailsContext;
        public EmployeeOffboardingController(EmployeeDetailsContext employeeDetailsContext)
        {
            this._empDetailsContext = employeeDetailsContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<EmployeeOffboard>>> getAllEmployeeOffboardings()
        {
            return Ok(await _empDetailsContext.EmployeeOffboards.ToListAsync());
        }
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeOffboard>> getEmployeeOffboardingsbyId(int employeeId)
        {
            var employeeOffboard = await _empDetailsContext.EmployeeOffboards.FindAsync(employeeId);
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
            _empDetailsContext.EmployeeOffboards.Add(employeeOffboard);
            await _empDetailsContext.SaveChangesAsync();
            return CreatedAtAction(
                nameof(getEmployeeOffboardingsbyId),
                new { employeeId = employeeOffboard.EmployeeId },
                employeeOffboard
            );
        }
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> updateEmployeeOffboardings(int employeeId, EmployeeOffboard employeeOffboard)
        {
            var employeeOffboardings = await _empDetailsContext.EmployeeOffboards.FindAsync(employeeId);
            if (employeeOffboardings == null)
            {
                return NotFound();
            }

            employeeOffboardings.EmployeeName = employeeOffboard.EmployeeName;
            employeeOffboardings.EmployeeEmail = employeeOffboard.EmployeeEmail;
            employeeOffboardings.EmployeeCode = employeeOffboard.EmployeeCode;
            employeeOffboardings.Designation = employeeOffboard.Designation;
            employeeOffboardings.Project = employeeOffboard.Project;
            employeeOffboardings.DateOfJoining = employeeOffboard.DateOfJoining;
            employeeOffboardings.Location = employeeOffboard.Location;
            employeeOffboardings.ResignationSubmittedDate = employeeOffboard.ResignationSubmittedDate;
            employeeOffboardings.LastWorkingDay = employeeOffboard.LastWorkingDay;
            employeeOffboardings.PanCardNumber = employeeOffboard.PanCardNumber;
            employeeOffboardings.BankAccountNumber = employeeOffboard.BankAccountNumber;
            employeeOffboardings.MobileNumber = employeeOffboard.MobileNumber;
            employeeOffboardings.ContactNumberResidence = employeeOffboard.ContactNumberResidence;
            employeeOffboardings.EmployeeAddress = employeeOffboard.EmployeeAddress;
            employeeOffboardings.EmploymentStatus = employeeOffboard.EmploymentStatus;

            await _empDetailsContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> deleteEmployeeOffboardings(int employeeId)
        {
            var employeeOffboardings = await _empDetailsContext.EmployeeOffboards.FindAsync(employeeId);
            if (employeeOffboardings == null)
            {
                return NotFound();
            }
            _empDetailsContext.EmployeeOffboards.Remove(employeeOffboardings);
            await _empDetailsContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
