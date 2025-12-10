using System.Reflection.Metadata;
using FirstAPI.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeOffboardingController : ControllerBase
    {
        static private List<EmployeeOffboard> OffboardingEmployees = new List<EmployeeOffboard>
        {
            new EmployeeOffboard
            {
                EmployeeId = 1,
                employeeName = "Test",
                employeeEmail = "Test@gmail.com",
                lastWorkingDay = new DateOnly(2025, 12, 24),
                employeeCode = "EMP001",
                designation = "Developer",
                project = "Project A",
                dateOfJoining = new DateOnly(2020, 01, 15),
                location = "Chennai",
                resignationSubmittedDate = new DateOnly(2025, 12, 01),
                panCardNumber = "ABCDE1234F",
                bankAccountNumber = "1234567890",
                employeeAddress = "123 Main Street",
                contactNumberResidence = "0441234567",
                mobileNumber = "9876543210",
                employmentStatus = "Active"
            }


        };
        [HttpGet]
        public ActionResult<List<EmployeeOffboard>> GetOffboardingEmployees()
        {
            return Ok(OffboardingEmployees);
        }
        [HttpGet("{id}")]
        public ActionResult<List<EmployeeOffboard>> GetOffboardingEmployeesbyId(int id)
        {
            var employeeOffboard = OffboardingEmployees.FirstOrDefault(empoff => empoff.EmployeeId == id);
            if (employeeOffboard == null)
                return NotFound();
            return Ok(employeeOffboard);
        }
        [HttpPost]
        public ActionResult<EmployeeOffboard> AddOffboardingEmployees(EmployeeOffboard empOffboard)
        {
            if (empOffboard == null)
            {
                return BadRequest();
            }
            OffboardingEmployees.Add(empOffboard);
            return CreatedAtAction(nameof(GetOffboardingEmployeesbyId), new { id = empOffboard.EmployeeId }, empOffboard);
        }
    }
}
