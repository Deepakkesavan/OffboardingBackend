namespace offboarding_prc_api.DTOs;
using System.Text.Json;

// ── EMPLOYEE ─────────────────────────────────────────────────────
public record EmployeeDto(
    string Id,
    string Name,
    string Designation,
    string Department,
    string Email,
    string? Manager
);

// ── OFFBOARDING RECORD ───────────────────────────────────────────
public record CreateRecordRequest(
    string EmployeeId,
    string EmployeeName,
    string? Designation,
    string? Department,
    string? Email,
    string? Manager,
    string? ExitReason,
    string? ExitDetails,
    string? JobDescription,
    DateTime? EndDate,
    int NoticePeriod
);

public record UpdateRecordRequest(
    string? CurrentStage,
    string? Status,
    DateTime? EndDate,
    string? ExitReason
);

public record OffboardingRecordDto(
    int Id,
    string EmployeeId,
    string EmployeeName,
    string? Designation,
    string? Department,
    string? Email,
    string? Manager,
    string? ExitReason,
    string? ExitDetails,
    string? JobDescription,
    DateTime? EndDate,
    int NoticePeriod,
    string CurrentStage,
    string Status,
    DateTime SubmittedAt
);

// ── STAGE DATA ───────────────────────────────────────────────────
public record CreateStageDataRequest(
    int RecordId,
    string StageType,
    JsonElement? Payload,    // JsonElement accepts any JSON value (object, string, null)
    DateTime? CompletedAt
);

public record UpdateStageDataRequest(
    JsonElement? Payload,
    DateTime? CompletedAt
);

public record StageDataDto(
    int Id,
    int RecordId,
    string StageType,
    string Payload,
    DateTime? CompletedAt
);

// ── CLEARANCE ────────────────────────────────────────────────────
public record CreateClearanceRequest(
    int RecordId,
    string Department,
    bool IsUnlocked,
    bool IsCleared
);

public record UpdateClearanceRequest(
    bool? IsUnlocked,
    bool? IsCleared,
    string? ClearedBy,
    DateTime? ClearedAt,
    string? Notes
);

public record ClearanceDto(
    int Id,
    int RecordId,
    string Department,
    bool IsUnlocked,
    bool IsCleared,
    string? ClearedBy,
    DateTime? ClearedAt,
    string? Notes
);

// ── AUDIT LOG ────────────────────────────────────────────────────
public record CreateAuditRequest(
    int RecordId,
    string Action,
    string? PerformedBy,
    string? StageBefore,
    string? StageAfter
);

public record AuditEntryDto(
    int Id,
    int RecordId,
    string Action,
    string? PerformedBy,
    string? StageBefore,
    string? StageAfter,
    DateTime Timestamp
);

// ── NOTICE PERIOD (computed) ─────────────────────────────────────
public record NoticePeriodDto(
    DateTime SubmittedAt,
    DateTime NoticeEndDate,   // SubmittedAt + 90 days
    int DaysRemaining
);
