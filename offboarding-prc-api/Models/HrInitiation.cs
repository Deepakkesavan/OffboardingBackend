namespace offboarding_prc_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ─────────────────────────────────────────────────────────────────
//  HR INITIATION
//  Stored in the [off].[HrInitiations] table.
//  Created when HR clicks "Initiate HR Process" on the HR Employee
//  Details screen, after the reporting manager has already approved
//  the resignation (SubmissionLog.StageAfter == "manager_approved").
//
//  Mirrors the ManagerApproval pattern: one durable, auditable row
//  per HR-initiation action, used both to advance the stage chain
//  and to make the action idempotent (a second click is rejected,
//  same as duplicate manager approvals).
// ─────────────────────────────────────────────────────────────────
[Table("HrInitiations", Schema = "off")]
public class HrInitiation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // The SubmissionLog row this initiation relates to
    [Required]
    public Guid SubmissionLogId { get; set; }

    // The employee being moved into HR initiation
    [Required, MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    // The HR user who initiated
    [Required, MaxLength(50)]
    public string HrEmpId { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? HrName { get; set; }

    // Free-text notes HR entered when initiating
    public string? HrComments { get; set; }

    // Snapshot fields (denormalised so the record is self-contained)
    [MaxLength(200)]
    public string? EmployeeName { get; set; }

    [MaxLength(200)]
    public string? Designation { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    public DateTime? LastWorkingDay { get; set; }

    // Audit
    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}