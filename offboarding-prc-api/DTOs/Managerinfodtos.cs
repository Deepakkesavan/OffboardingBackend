namespace offboarding_prc_api.DTOs;
using offboarding_prc_api.Models;

// ── Team member summary (subset of EmpInfo fields, safe to expose) ─
public record TeamMemberDto(
    string EmpId,
    string FullName,
    string Desg,
    string? Project,
    string? Grade,
    string? Email,
    string? Gender,
    string? Doj,
    bool IsOffboarding   // true if empId exists in SubmissionLogs (isActive = true)
);

// ── Full manager info response ──────────────────────────────────────
public record ManagerInfoResponse(
    // Logged-in user details
    string EmpId,
    string FullName,
    string Desg,
    string? Email,
    string? Project,
    string? Grade,
    string? ManagerEmpCode,
    string? ReportingManager,

    // Reporting-manager flag and team stats
    bool IsReportingManager,
    string NoOfTotalMembers,      // numeric as string per spec
    string NoOfActive,
    string NoOfOffboarding,

    // Full team list
    List<TeamMemberDto> TotalMembers
);