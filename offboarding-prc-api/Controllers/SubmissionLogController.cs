namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Services;

[ApiController]
[Route("api/submission")]
public class SubmissionLogController(SubmissionLogService submissionLogService) : ControllerBase
{
    // POST api/submission/submit
    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitActionRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.EmployeeId))
            return BadRequest(new { message = "EmployeeId is required." });

        if (string.IsNullOrWhiteSpace(req.Action))
            return BadRequest(new { message = "Action is required." });

        var (_, response) = await submissionLogService.SaveAsync(req);

        return StatusCode(201, response);
    }

    // GET api/submission/getsubmit?employeeId=EMP001
    [HttpGet("getsubmit")]
    public async Task<IActionResult> GetSubmit([FromQuery] string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            return BadRequest(new { message = "employeeId query param is required." });

        var response = await submissionLogService.GetByEmployeeIdAsync(employeeId);
        return Ok(response);
    }

    // ── GET api/submission/all ──────────────────────────────────────
    // Returns every active SubmissionLog row across ALL employees, newest
    // first. Used by the HR Dashboard (Recent Activity feed + Offboarding
    // Records table) — unlike GetSubmit, this is not scoped to one employee.
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var response = await submissionLogService.GetAllAsync();
        return Ok(response);
    }
}