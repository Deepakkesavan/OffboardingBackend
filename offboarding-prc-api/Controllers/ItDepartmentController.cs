namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;
using offboarding_prc_api.Services;

// ─────────────────────────────────────────────────────────────────
//  IT DEPARTMENT CONTROLLER
//
//  POST /api/ItDepartment
//    - IT staff submits clearance from the IT Employee Details screen
//    - Only allowed once HR has initiated (SubmissionLog.StageAfter == "hr_initiation")
//    - Saves to off.ItClearances
//    - Advances SubmissionLog stage to 'department_clearance'
//
//  GET  /api/GetItClearance/{submissionLogId}
//    - Returns the IT clearance record for a given SubmissionLog GUID
//    - Frontend uses this to show approved state vs. action form
//
//  GET  /api/ItDepartment/all
//    - Returns every active ItClearance row (for potential future dashboards)
// ─────────────────────────────────────────────────────────────────
[ApiController]
public class ItDepartmentController(AppDbContext db, SubmissionLogService submissionLogService) : ControllerBase
{
    // ── POST /api/ItDepartment ──────────────────────────────────────
    [HttpPost("api/ItDepartment")]
    public async Task<IActionResult> Approve([FromBody] ItClearanceRequest req)
    {
        if (req.SubmissionLogId == Guid.Empty)
            return BadRequest(new { message = "SubmissionLogId is required." });

        if (string.IsNullOrWhiteSpace(req.EmployeeId))
            return BadRequest(new { message = "EmployeeId is required." });

        if (string.IsNullOrWhiteSpace(req.ItEmpId))
            return BadRequest(new { message = "ItEmpId is required." });

        // Prevent duplicate clearances for the same submission
        bool alreadyCleared = await db.ItClearances
            .AnyAsync(it => it.SubmissionLogId == req.SubmissionLogId && it.IsActive);

        if (alreadyCleared)
            return Conflict(new { message = "IT clearance has already been recorded for this offboarding." });

        // Verify the SubmissionLog exists and has been HR-initiated.
        var submissionLog = await db.SubmissionLogs
            .FirstOrDefaultAsync(s => s.Id == req.SubmissionLogId && s.IsActive);

        if (submissionLog is null)
            return NotFound(new { message = $"SubmissionLog {req.SubmissionLogId} not found or inactive." });

        if (submissionLog.StageAfter != "hr_initiation")
            return Conflict(new
            {
                message = "This offboarding has not been through HR initiation yet."
            });

        // Save clearance record
        var clearance = new ItClearance
        {
            SubmissionLogId = req.SubmissionLogId,
            EmployeeId = req.EmployeeId,
            ItEmpId = req.ItEmpId,
            ItName = req.ItName,

            CorporateLaptopReturned = req.CorporateLaptopReturned,
            MobileDeviceReturned = req.MobileDeviceReturned,
            SecurityBadgeReturned = req.SecurityBadgeReturned,
            AccessCardsReturned = req.AccessCardsReturned,

            CorporateEmailStatus = req.CorporateEmailStatus,
            CloudInfraStatus = req.CloudInfraStatus,
            VpnAccessStatus = req.VpnAccessStatus,
            InternalToolsStatus = req.InternalToolsStatus,

            DeviceSerialNumber = req.DeviceSerialNumber,
            SecondaryAssetNotes = req.SecondaryAssetNotes,
            ItComments = req.ItComments,

            EmployeeName = req.EmployeeName,
            Designation = req.Designation,
            Department = req.Department,

            ClearedAt = DateTime.UtcNow,
            IsActive = true,
        };

        db.ItClearances.Add(clearance);
        await db.SaveChangesAsync();

        // ── Advance the SubmissionLog stage chain ───────────────
        await submissionLogService.AdvanceStageAsync(req.EmployeeId, "department_clearance");

        return StatusCode(201, ToDto(clearance));
    }

    // ── GET /api/GetItClearance/{submissionLogId} ──────────────────
    [HttpGet("api/GetItClearance/{submissionLogId:guid}")]
    public async Task<IActionResult> GetBySubmissionLog(Guid submissionLogId)
    {
        var clearance = await db.ItClearances
            .Where(it => it.SubmissionLogId == submissionLogId && it.IsActive)
            .OrderByDescending(it => it.ClearedAt)
            .FirstOrDefaultAsync();

        if (clearance is null)
            return Ok(new { isCleared = false });

        return Ok(new
        {
            isCleared = true,
            data = ToDto(clearance),
        });
    }

    // ── GET /api/ItDepartment/all ────────────────────────────────────
    [HttpGet("api/ItDepartment/all")]
    public async Task<IActionResult> GetAll()
    {
        var clearances = await db.ItClearances
            .Where(it => it.IsActive)
            .OrderByDescending(it => it.ClearedAt)
            .ToListAsync();

        return Ok(clearances.Select(ToDto));
    }

    // ── Helper ────────────────────────────────────────────────────
    private static ItClearanceDto ToDto(ItClearance it) => new(
        it.Id,
        it.SubmissionLogId,
        it.EmployeeId,
        it.ItEmpId,
        it.ItName,
        it.CorporateLaptopReturned,
        it.MobileDeviceReturned,
        it.SecurityBadgeReturned,
        it.AccessCardsReturned,
        it.CorporateEmailStatus,
        it.CloudInfraStatus,
        it.VpnAccessStatus,
        it.InternalToolsStatus,
        it.DeviceSerialNumber,
        it.SecondaryAssetNotes,
        it.ItComments,
        it.EmployeeName,
        it.Designation,
        it.Department,
        it.ClearedAt,
        it.IsActive
    );
}