namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;

// ─────────────────────────────────────────────────────────────────
//  AUDIT LOG
// ─────────────────────────────────────────────────────────────────
[ApiController]
[Route("api/audit-log")]
public class AuditLogController(AppDbContext db) : ControllerBase
{
    // GET api/audit-log?recordId=1
    [HttpGet]
    public async Task<IActionResult> GetByRecord([FromQuery] int recordId)
    {
        var logs = await db.AuditLog
            .Where(a => a.RecordId == recordId)
            .OrderBy(a => a.Timestamp)
            .Select(a => new AuditEntryDto(
                a.Id, a.RecordId, a.Action, a.PerformedBy,
                a.StageBefore, a.StageAfter, a.Timestamp))
            .ToListAsync();

        return Ok(logs);
    }

    // POST api/audit-log
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuditRequest req)
    {
        var entry = new AuditEntry
        {
            RecordId = req.RecordId,
            Action = req.Action,
            PerformedBy = req.PerformedBy,
            StageBefore = req.StageBefore,
            StageAfter = req.StageAfter,
            Timestamp = DateTime.UtcNow,
        };

        db.AuditLog.Add(entry);
        await db.SaveChangesAsync();

        return Ok(new AuditEntryDto(
            entry.Id, entry.RecordId, entry.Action, entry.PerformedBy,
            entry.StageBefore, entry.StageAfter, entry.Timestamp));
    }
}