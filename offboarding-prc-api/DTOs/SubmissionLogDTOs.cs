namespace offboarding_prc_api.DTOs;
using System.Text.Json;

// ── SUBMISSION LOG — Incoming request ────────────────────────────
public record SubmitActionRequest(
    string EmployeeId,
    string Action,
    string? PerformedBy,
    JsonElement? EmployeeData,
    string? StageBefore,
    string? StageAfter
);

// ── SUBMISSION LOG — POST response ───────────────────────────────
public record SubmitActionResponse(
    string StatusCode,
    string Message,
    TimeOnly Time,
    DateOnly Date
);

// ── SUBMISSION LOG — GET response ────────────────────────────────
// Returned by GET /api/submission/getsubmit?employeeId=...
public record GetSubmitResponse(
    bool IsSubmitted,
    string? EmployeeId,
    string? Action,
    string? PerformedBy,
    string? StageBefore,
    string? StageAfter,
    TimeOnly? Time,
    DateOnly? Date
);