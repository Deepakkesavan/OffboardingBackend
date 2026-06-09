namespace offboarding_prc_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offboarding_prc_api.Data;
using offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;
using System.Text.Json;

[ApiController]
[Route("api/stage-data")]
public class StageDataController(AppDbContext db) : ControllerBase
{
    // ── GET api/stage-data?recordId=1 ────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetByRecord(
        [FromQuery] int recordId,
        [FromQuery] string? stageType = null)
    {
        var query = db.StageDataList.Where(s => s.RecordId == recordId);

        if (stageType is not null)
            query = query.Where(s => s.StageType == stageType);

        var results = await query.OrderBy(s => s.Id).ToListAsync();

        // Return payload as a parsed JSON object so React receives an object, not a string
        var response = results.Select(s => new
        {
            id = s.Id,
            recordId = s.RecordId,
            stageType = s.StageType,
            payload = ParsePayload(s.Payload),
            completedAt = s.CompletedAt,
        });

        return Ok(response);
    }

    // ── GET api/stage-data/{id} ──────────────────────────────────
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await db.StageDataList.FindAsync(id);
        if (s is null) return NotFound();

        return Ok(new
        {
            id = s.Id,
            recordId = s.RecordId,
            stageType = s.StageType,
            payload = ParsePayload(s.Payload),
            completedAt = s.CompletedAt,
        });
    }

    // ── POST api/stage-data ──────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStageDataRequest req)
    {
        // Prevent duplicate stage rows for the same record+stage
        bool exists = await db.StageDataList
            .AnyAsync(s => s.RecordId == req.RecordId && s.StageType == req.StageType);

        if (exists)
            return Conflict(new { message = $"Stage '{req.StageType}' already exists for record {req.RecordId}." });

        var stage = new StageData
        {
            RecordId = req.RecordId,
            StageType = req.StageType,
            Payload = SerializePayload(req.Payload),
            CompletedAt = req.CompletedAt,
        };

        db.StageDataList.Add(stage);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = stage.Id }, new
        {
            id = stage.Id,
            recordId = stage.RecordId,
            stageType = stage.StageType,
            payload = ParsePayload(stage.Payload),
            completedAt = stage.CompletedAt,
        });
    }

    // ── PATCH api/stage-data/{id} ────────────────────────────────
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStageDataRequest req)
    {
        var stage = await db.StageDataList.FindAsync(id);
        if (stage is null) return NotFound();

        if (req.Payload.HasValue)
            stage.Payload = SerializePayload(req.Payload);

        if (req.CompletedAt is not null)
            stage.CompletedAt = req.CompletedAt;

        await db.SaveChangesAsync();

        return Ok(new
        {
            id = stage.Id,
            recordId = stage.RecordId,
            stageType = stage.StageType,
            payload = ParsePayload(stage.Payload),
            completedAt = stage.CompletedAt,
        });
    }

    // ── Helpers ──────────────────────────────────────────────────

    // Serialize JsonElement → stored string in DB
    private static string SerializePayload(System.Text.Json.JsonElement? element)
    {
        if (element is null || !element.HasValue)
            return "{}";

        return element.Value.ValueKind == JsonValueKind.Null
            ? "{}"
            : element.Value.GetRawText();
    }

    // Parse stored string → object for JSON response (React gets a real object, not a string)
    private static object ParsePayload(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new { };
        try
        {
            return JsonSerializer.Deserialize<JsonElement>(raw);
        }
        catch
        {
            return new { };
        }
    }
}