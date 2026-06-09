namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;
using offboarding_prc_api.Services;


[ApiController]
[Route("api/offboarding-records")]
public class OffboardingRecordsController(
    AppDbContext db,
    StageGateService stageGate,
    NoticePeriodService noticePeriod) : ControllerBase
{
    // ── GET api/offboarding-records ──────────────────────────────
    // Returns all records (used by Dashboard + HR Queue)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await db.OffboardingRecords
            .OrderByDescending(r => r.SubmittedAt)
            .Select(r => ToDto(r))
            .ToListAsync();

        return Ok(records);
    }

    // ── GET api/offboarding-records/{id} ────────────────────────
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var record = await db.OffboardingRecords.FindAsync(id);
        if (record is null) return NotFound(new { message = $"Record {id} not found." });

        return Ok(ToDto(record));
    }

    // ── POST api/offboarding-records ────────────────────────────
    // Called from NewRequest.jsx when the user fills the initial form
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecordRequest req)
    {
        var record = new OffboardingRecord
        {
            EmployeeId = req.EmployeeId,
            EmployeeName = req.EmployeeName,
            Designation = req.Designation,
            Department = req.Department,
            Email = req.Email,
            Manager = req.Manager,
            ExitReason = req.ExitReason,
            ExitDetails = req.ExitDetails,
            JobDescription = req.JobDescription,
            EndDate = req.EndDate,
            NoticePeriod = req.NoticePeriod,
            CurrentStage = "exit_interview",
            Status = "active",
            SubmittedAt = DateTime.UtcNow,
        };

        db.OffboardingRecords.Add(record);
        await db.SaveChangesAsync();

        // Automatically create the exit_interview StageData row (empty payload, not yet completed)
        db.StageDataList.Add(new StageData
        {
            RecordId = record.Id,
            StageType = "exit_interview",
            Payload = "{}",
            CompletedAt = null,
        });

        // Audit entry
        db.AuditLog.Add(new AuditEntry
        {
            RecordId = record.Id,
            Action = "record_created",
            PerformedBy = req.EmployeeName,
            StageBefore = null,
            StageAfter = "exit_interview",
            Timestamp = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = record.Id }, ToDto(record));
    }

    // ── PATCH api/offboarding-records/{id} ──────────────────────
    // Called when stage transitions happen (e.g. manager approval sets
    // CurrentStage = "hr_initiation")
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecordRequest req)
    {
        var record = await db.OffboardingRecords.FindAsync(id);
        if (record is null) return NotFound(new { message = $"Record {id} not found." });

        // Stage transition gate: if a new stage is requested, validate it
        if (req.CurrentStage is not null && req.CurrentStage != record.CurrentStage)
        {
            if (!StageGateService.IsValidStage(req.CurrentStage))
                return BadRequest(new { message = $"'{req.CurrentStage}' is not a valid stage." });

            bool canAdvance = await stageGate.CanAdvanceAsync(id, req.CurrentStage);
            if (!canAdvance)
                return Conflict(new { message = "Cannot advance: previous stage not yet completed." });

            record.CurrentStage = req.CurrentStage;
        }

        if (req.Status is not null) record.Status = req.Status;
        if (req.EndDate is not null) record.EndDate = req.EndDate;
        if (req.ExitReason is not null) record.ExitReason = req.ExitReason;

        await db.SaveChangesAsync();
        return Ok(ToDto(record));
    }

    // ── GET api/offboarding-records/{id}/notice-period ──────────
    // Returns the 90-day notice period calculation for HR Final Approval
    [HttpGet("{id:int}/notice-period")]
    public async Task<IActionResult> GetNoticePeriod(int id)
    {
        var record = await db.OffboardingRecords.FindAsync(id);
        if (record is null) return NotFound(new { message = $"Record {id} not found." });

        var (noticeEndDate, daysRemaining) = noticePeriod.Calculate(record.SubmittedAt, record.NoticePeriod);

        return Ok(new NoticePeriodDto(record.SubmittedAt, noticeEndDate, daysRemaining));
    }

    // ── Mapping helper ───────────────────────────────────────────
    private static OffboardingRecordDto ToDto(OffboardingRecord r) => new(
        r.Id, r.EmployeeId, r.EmployeeName, r.Designation, r.Department,
        r.Email, r.Manager, r.ExitReason, r.ExitDetails, r.JobDescription,
        r.EndDate, r.NoticePeriod, r.CurrentStage, r.Status, r.SubmittedAt
    );
}
