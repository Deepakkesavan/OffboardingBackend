namespace offboarding_prc_api.DTOs;

// ── Incoming request from the IT Employee Details "Approve IT Clearance" action ──
public record ItClearanceRequest(
    Guid SubmissionLogId,
    string EmployeeId,
    string ItEmpId,
    string? ItName,

    // Asset checklist
    bool CorporateLaptopReturned,
    bool MobileDeviceReturned,
    bool SecurityBadgeReturned,
    bool AccessCardsReturned,

    // Access revocation statuses: "suspended" | "revoked" | "pending"
    string? CorporateEmailStatus,
    string? CloudInfraStatus,
    string? VpnAccessStatus,
    string? InternalToolsStatus,

    // Asset verification
    string? DeviceSerialNumber,
    string? SecondaryAssetNotes,

    // Final comments
    string? ItComments,

    // Snapshot fields
    string? EmployeeName,
    string? Designation,
    string? Department
);

// ── Response returned after clearance or on GET ───────────────────
public record ItClearanceDto(
    Guid Id,
    Guid SubmissionLogId,
    string EmployeeId,
    string ItEmpId,
    string? ItName,

    bool CorporateLaptopReturned,
    bool MobileDeviceReturned,
    bool SecurityBadgeReturned,
    bool AccessCardsReturned,

    string? CorporateEmailStatus,
    string? CloudInfraStatus,
    string? VpnAccessStatus,
    string? InternalToolsStatus,

    string? DeviceSerialNumber,
    string? SecondaryAssetNotes,
    string? ItComments,

    string? EmployeeName,
    string? Designation,
    string? Department,
    DateTime ClearedAt,
    bool IsActive
);