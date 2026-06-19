namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;
using offboarding_prc_api.Services;

// ─────────────────────────────────────────────────────────────────
//  APPROVE OFFBOARDING CONTROLLER
//
//  POST /api/ApproveOffboarding
//    - Manager submits approval with comments
//    - Saves to off.ManagerApprovals
//    - Updates SubmissionLog.StageBefore/StageAfter to 'manager_approved'
//      via SubmissionLogService.AdvanceStageAsync, keeping the stage
//      chain accurate for every later GetSubmit lookup.
//
//  GET  /api/GetApproveOffboarding/{submissionLogId}
//    - Returns the approval record for a given SubmissionLog GUID
//    - Frontend uses this to decide whether to show approved card
//
//  GET  /api/ApproveOffboarding/all
//    - Returns every active ManagerApproval row across ALL employees
//    - Used by the HR Dashboard to source LastWorkingDay per employee
// ─────────────────────────────────────────────────────────────────
[ApiController]
public class ApproveOffboardingController(AppDbContext db, SubmissionLogService submissionLogService) : ControllerBase
{
    // ── POST /api/ApproveOffboarding ──────────────────────────────
    [HttpPost("api/ApproveOffboarding")]
    public async Task<IActionResult> Approve([FromBody] ApproveOffboardingRequest req)
    {
        if (req.SubmissionLogId == Guid.Empty)
            return BadRequest(new { message = "SubmissionLogId is required." });

        if (string.IsNullOrWhiteSpace(req.EmployeeId))
            return BadRequest(new { message = "EmployeeId is required." });

        // Prevent duplicate approvals for the same submission
        bool alreadyApproved = await db.ManagerApprovals
            .AnyAsync(ma => ma.SubmissionLogId == req.SubmissionLogId && ma.IsActive);

        if (alreadyApproved)
            return Conflict(new { message = "This resignation has already been approved by the manager." });

        // Verify the SubmissionLog exists
        var submissionLog = await db.SubmissionLogs
            .FirstOrDefaultAsync(s => s.Id == req.SubmissionLogId && s.IsActive);

        if (submissionLog is null)
            return NotFound(new { message = $"SubmissionLog {req.SubmissionLogId} not found or inactive." });

        // Save approval record
        var approval = new ManagerApproval
        {
            SubmissionLogId = req.SubmissionLogId,
            EmployeeId = req.EmployeeId,
            ManagerEmpId = req.ManagerEmpId,
            ManagerName = req.ManagerName,
            ManagerComments = req.ManagerComments,
            EmployeeName = req.EmployeeName,
            Designation = req.Designation,
            Department = req.Department,
            ResignationDate = req.ResignationDate,
            LastWorkingDay = req.LastWorkingDay,
            ReasonForLeaving = req.ReasonForLeaving,
            ApprovedAt = DateTime.UtcNow,
            IsActive = true,
        };

        db.ManagerApprovals.Add(approval);
        await db.SaveChangesAsync();

        // ── Advance the SubmissionLog stage chain ───────────────
        // StageBefore is set to whatever StageAfter currently is,
        // then StageAfter moves to 'manager_approved'.
        await submissionLogService.AdvanceStageAsync(req.EmployeeId, "manager_approved");

        return StatusCode(201, ToDto(approval));
    }

    // ── GET /api/GetApproveOffboarding/{submissionLogId} ──────────
    [HttpGet("api/GetApproveOffboarding/{submissionLogId:guid}")]
    public async Task<IActionResult> GetBySubmissionLog(Guid submissionLogId)
    {
        var approval = await db.ManagerApprovals
            .Where(ma => ma.SubmissionLogId == submissionLogId && ma.IsActive)
            .OrderByDescending(ma => ma.ApprovedAt)
            .FirstOrDefaultAsync();

        if (approval is null)
            return Ok(new { isApproved = false });

        return Ok(new
        {
            isApproved = true,
            data = ToDto(approval),
        });
    }

    // ── GET /api/ApproveOffboarding/all ─────────────────────────────
    // Returns every active ManagerApproval row across ALL employees,
    // newest first. The HR Dashboard joins this against SubmissionLogs
    // (by EmployeeId) to source each offboarding employee's LastWorkingDay.
    [HttpGet("api/ApproveOffboarding/all")]
    public async Task<IActionResult> GetAll()
    {
        var approvals = await db.ManagerApprovals
            .Where(ma => ma.IsActive)
            .OrderByDescending(ma => ma.ApprovedAt)
            .ToListAsync();

        return Ok(approvals.Select(ToDto));
    }

    // ── Helper ────────────────────────────────────────────────────
    private static ManagerApprovalDto ToDto(ManagerApproval ma) => new(
        ma.Id,
        ma.SubmissionLogId,
        ma.EmployeeId,
        ma.ManagerEmpId,
        ma.ManagerName,
        ma.ManagerComments,
        ma.EmployeeName,
        ma.Designation,
        ma.Department,
        ma.ResignationDate,
        ma.LastWorkingDay,
        ma.ReasonForLeaving,
        ma.ApprovedAt,
        ma.IsActive
    );
}