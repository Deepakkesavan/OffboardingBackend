namespace offboarding_prc_api.DTOs;

// ── Incoming request from the HR Employee Details "Initiate HR Process" action ──
public record HrInitiationRequest(
    Guid SubmissionLogId,
    string EmployeeId,
    string HrEmpId,
    string? HrName,
    string? HrComments,
    // Snapshot fields
    string? EmployeeName,
    string? Designation,
    string? Department,
    DateTime? LastWorkingDay
);

// ── Response returned after initiation or on GET ─────────────────
public record HrInitiationDto(
    Guid Id,
    Guid SubmissionLogId,
    string EmployeeId,
    string HrEmpId,
    string? HrName,
    string? HrComments,
    string? EmployeeName,
    string? Designation,
    string? Department,
    DateTime? LastWorkingDay,
    DateTime InitiatedAt,
    bool IsActive
);