namespace offboarding_prc_api.DTOs;
using System.Text.Json;

// ── SUBMISSION LOG — Incoming request ────────────────────────────
// The caller sends only these fields; the rest are generated server-side.
public record SubmitActionRequest(
    string EmployeeId,
    string Action,
    string? PerformedBy,
    JsonElement? EmployeeData,   // accepts any JSON shape
    string? StageBefore,
    string? StageAfter
);

// ── SUBMISSION LOG — Response ─────────────────────────────────────
// Returned after a successful POST /submit
public record SubmitActionResponse(
    string StatusCode,
    string Message,
    TimeOnly Time,       // time portion of CreatedAt (UTC)
    DateOnly Date        // date portion of CreatedAt (UTC)
);