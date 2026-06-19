namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;
using offboarding_prc_api.Services;

// ─────────────────────────────────────────────────────────────────
//  HR INITIATION CONTROLLER
//
//  POST /api/HRInitiation
//    - HR submits initiation (with optional comments) from the
//      HR Employee Details screen
//    - Only allowed once the employee's latest SubmissionLog stage
//      is "manager_approved" — enforced here, not just in the UI
//    - Saves to off.HrInitiations
//    - Updates SubmissionLog.StageBefore/StageAfter to 'hr_initiation'
//      via SubmissionLogService.AdvanceStageAsync
//
//  GET  /api/GetHRInitiation/{submissionLogId}
//    - Returns the initiation record for a given SubmissionLog GUID
//    - Frontend uses this to decide whether to show the
//      already-initiated state vs. the action button
// ─────────────────────────────────────────────────────────────────
[ApiController]
public class HrInitiationController(AppDbContext db, SubmissionLogService submissionLogService) : ControllerBase
{
    // ── POST /api/HRInitiation ──────────────────────────────────────
    [HttpPost("api/HRInitiation")]
    public async Task<IActionResult> Initiate([FromBody] HrInitiationRequest req)
    {
        if (req.SubmissionLogId == Guid.Empty)
            return BadRequest(new { message = "SubmissionLogId is required." });

        if (string.IsNullOrWhiteSpace(req.EmployeeId))
            return BadRequest(new { message = "EmployeeId is required." });

        if (string.IsNullOrWhiteSpace(req.HrEmpId))
            return BadRequest(new { message = "HrEmpId is required." });

        // Prevent duplicate initiations for the same submission
        bool alreadyInitiated = await db.HrInitiations
            .AnyAsync(hi => hi.SubmissionLogId == req.SubmissionLogId && hi.IsActive);

        if (alreadyInitiated)
            return Conflict(new { message = "HR initiation has already been recorded for this offboarding." });

        // Verify the SubmissionLog exists and has been manager-approved.
        // The stage check guards against HR jumping ahead before the
        // reporting manager has signed off.
        var submissionLog = await db.SubmissionLogs
            .FirstOrDefaultAsync(s => s.Id == req.SubmissionLogId && s.IsActive);

        if (submissionLog is null)
            return NotFound(new { message = $"SubmissionLog {req.SubmissionLogId} not found or inactive." });

        if (submissionLog.StageAfter != "manager_approved")
            return Conflict(new
            {
                message = "This offboarding has not been approved by the reporting manager yet."
            });

        // Save initiation record
        var initiation = new HrInitiation
        {
            SubmissionLogId = req.SubmissionLogId,
            EmployeeId = req.EmployeeId,
            HrEmpId = req.HrEmpId,
            HrName = req.HrName,
            HrComments = req.HrComments,
            EmployeeName = req.EmployeeName,
            Designation = req.Designation,
            Department = req.Department,
            LastWorkingDay = req.LastWorkingDay,
            InitiatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        db.HrInitiations.Add(initiation);
        await db.SaveChangesAsync();

        // ── Advance the SubmissionLog stage chain ───────────────
        await submissionLogService.AdvanceStageAsync(req.EmployeeId, "hr_initiation");

        return StatusCode(201, ToDto(initiation));
    }

    // ── GET /api/GetHRInitiation/{submissionLogId} ──────────────────
    [HttpGet("api/GetHRInitiation/{submissionLogId:guid}")]
    public async Task<IActionResult> GetBySubmissionLog(Guid submissionLogId)
    {
        var initiation = await db.HrInitiations
            .Where(hi => hi.SubmissionLogId == submissionLogId && hi.IsActive)
            .OrderByDescending(hi => hi.InitiatedAt)
            .FirstOrDefaultAsync();

        if (initiation is null)
            return Ok(new { isInitiated = false });

        return Ok(new
        {
            isInitiated = true,
            data = ToDto(initiation),
        });
    }

    // ── Helper ────────────────────────────────────────────────────
    private static HrInitiationDto ToDto(HrInitiation hi) => new(
        hi.Id,
        hi.SubmissionLogId,
        hi.EmployeeId,
        hi.HrEmpId,
        hi.HrName,
        hi.HrComments,
        hi.EmployeeName,
        hi.Designation,
        hi.Department,
        hi.LastWorkingDay,
        hi.InitiatedAt,
        hi.IsActive
    );
}