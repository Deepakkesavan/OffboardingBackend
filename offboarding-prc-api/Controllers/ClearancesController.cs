namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;
using offboarding_prc_api.Services;

[ApiController]
[Route("api/clearances")]
public class ClearancesController(AppDbContext db, ClearanceService clearanceService) : ControllerBase
{
    // ── GET api/clearances?recordId=1 ────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetByRecord([FromQuery] int recordId)
    {
        // Every time clearances are fetched, check if Finance should be unlocked
        // (This is the lightweight alternative to a scheduled job for Phase 2)
        await clearanceService.UnlockFinanceClearancesAsync();

        var clearances = await db.Clearances
            .Where(c => c.RecordId == recordId)
            .OrderBy(c => c.Id)
            .Select(c => ToDto(c))
            .ToListAsync();

        return Ok(clearances);
    }

    // ── GET api/clearances/{id} ──────────────────────────────────
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await db.Clearances.FindAsync(id);
        if (c is null) return NotFound();
        return Ok(ToDto(c));
    }

    // ── POST api/clearances ──────────────────────────────────────
    // Called by HRInitiation when offboarding is started —
    // creates one row per department.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClearanceRequest req)
    {
        var clearance = new Clearance
        {
            RecordId = req.RecordId,
            Department = req.Department,
            IsUnlocked = req.IsUnlocked,
            IsCleared = req.IsCleared,
        };

        db.Clearances.Add(clearance);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = clearance.Id }, ToDto(clearance));
    }

    // ── PATCH api/clearances/{id} ────────────────────────────────
    // Called when a dept stakeholder marks their clearance done.
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClearanceRequest req)
    {
        var clearance = await db.Clearances.FindAsync(id);
        if (clearance is null) return NotFound();

        // Guard: Finance clearance cannot be marked cleared while still locked
        if (clearance.Department == "Finance" && !clearance.IsUnlocked && req.IsCleared == true)
            return BadRequest(new { message = "Finance clearance is locked until T-2 days before end date." });

        if (req.IsUnlocked is not null) clearance.IsUnlocked = req.IsUnlocked.Value;
        if (req.IsCleared is not null) clearance.IsCleared = req.IsCleared.Value;
        if (req.ClearedBy is not null) clearance.ClearedBy = req.ClearedBy;
        if (req.ClearedAt is not null) clearance.ClearedAt = req.ClearedAt;
        if (req.Notes is not null) clearance.Notes = req.Notes;

        await db.SaveChangesAsync();
        return Ok(ToDto(clearance));
    }

    // ── GET api/clearances/all-cleared/{recordId} ────────────────
    // Returns whether all departments have cleared — used to gate
    // the advance to final_approval
    [HttpGet("all-cleared/{recordId:int}")]
    public async Task<IActionResult> AllCleared(int recordId)
    {
        bool allCleared = await clearanceService.AllClearedAsync(recordId);
        return Ok(new { allCleared });
    }

    private static ClearanceDto ToDto(Clearance c) =>
        new(c.Id, c.RecordId, c.Department, c.IsUnlocked, c.IsCleared, c.ClearedBy, c.ClearedAt, c.Notes);
}
