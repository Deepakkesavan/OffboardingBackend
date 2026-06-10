namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Services;

// ─────────────────────────────────────────────────────────────────
//  SUBMISSION LOG CONTROLLER
//  POST /api/submission/submit  — called when the user clicks Submit
//  on any offboarding stage form.
// ─────────────────────────────────────────────────────────────────
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

        // Return 201 Created with the shaped response body
        return StatusCode(201, response);
    }
}