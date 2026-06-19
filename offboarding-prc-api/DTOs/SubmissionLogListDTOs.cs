namespace offboarding_prc_api.DTOs;

// ── SUBMISSION LOG — GET ALL response ────────────────────────────
// Returned by GET /api/submission/all.
// One row per SubmissionLog entry (full history, not just latest-per-employee),
// used by the HR Dashboard's Recent Activity feed and stage lookups.
//
// EmployeeName is best-effort: SubmissionLog does not have a dedicated
// name column, so it is pulled out of the EmployeeData JSON blob when
// present (the InitiateExit submit form sends fullName there today).
// Frontend treats this as nullable and falls back to showing the EmployeeId.
public record SubmissionLogEntryDto(
    Guid Id,
    string EmployeeId,
    string? EmployeeName,
    string Action,
    string? PerformedBy,
    string? StageBefore,
    string? StageAfter,
    DateTime CreatedAt
);