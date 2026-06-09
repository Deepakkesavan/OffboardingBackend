namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;

[ApiController]
[Route("api/departments")]
public class DepartmentsController(AppDbContext db) : ControllerBase
{
    // GET api/departments
    // Returns the full list of clearance departments.
    // Used by the React frontend to populate department dropdowns
    // and by HR Initiation to know which clearance rows to create.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await db.Departments
            .OrderBy(d => d.Name)
            .Select(d => new
            {
                d.Id,
                d.Name
            })
            .ToListAsync();

        return Ok(departments);
    }

    // GET api/departments/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await db.Departments.FindAsync(id);
        if (dept is null)
            return NotFound(new { message = $"Department {id} not found." });

        return Ok(new { dept.Id, dept.Name });
    }
}
