namespace offboarding_prc_api.DTOs;

// ── Incoming request from the manager's approval form ─────────────
public record ApproveOffboardingRequest(
    Guid SubmissionLogId,
    string EmployeeId,
    string ManagerEmpId,
    string? ManagerName,
    string? ManagerComments,
    // Snapshot fields
    string? EmployeeName,
    string? Designation,
    string? Department,
    DateTime? ResignationDate,
    DateTime? LastWorkingDay,
    string? ReasonForLeaving
);

// ── Response returned after approval or on GET ────────────────────
public record ManagerApprovalDto(
    Guid Id,
    Guid SubmissionLogId,
    string EmployeeId,
    string ManagerEmpId,
    string? ManagerName,
    string? ManagerComments,
    string? EmployeeName,
    string? Designation,
    string? Department,
    DateTime? ResignationDate,
    DateTime? LastWorkingDay,
    string? ReasonForLeaving,
    DateTime ApprovedAt,
    bool IsActive
);