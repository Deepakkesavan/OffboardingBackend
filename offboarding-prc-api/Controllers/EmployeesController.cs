namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;

[ApiController]
[Route("api/employees")]
public class EmployeesController(AppDbContext db) : ControllerBase
{
    // GET api/employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await db.Employees
            .Select(e => new EmployeeDto(e.Id, e.Name, e.Designation, e.Department, e.Email, e.Manager))
            .ToListAsync();

        return Ok(employees);
    }

    // GET api/employees/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var e = await db.Employees.FindAsync(id);
        if (e is null) return NotFound(new { message = $"Employee {id} not found." });

        return Ok(new EmployeeDto(e.Id, e.Name, e.Designation, e.Department, e.Email, e.Manager));
    }
}
