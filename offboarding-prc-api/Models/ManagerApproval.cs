namespace offboarding_prc_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ─────────────────────────────────────────────────────────────────
//  MANAGER APPROVAL
//  Stored in the [off].[ManagerApprovals] table.
//  Created when the reporting manager clicks "Approve Offboarding"
//  on the Employee Record screen.
// ─────────────────────────────────────────────────────────────────
[Table("ManagerApprovals", Schema = "off")]
public class ManagerApproval
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // The SubmissionLog row this approval relates to
    [Required]
    public Guid SubmissionLogId { get; set; }

    // The employee being approved for offboarding
    [Required, MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    // The manager who approved
    [Required, MaxLength(50)]
    public string ManagerEmpId { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ManagerName { get; set; }

    // Free-text comments the manager entered
    public string? ManagerComments { get; set; }

    // Snapshot fields (denormalised so the record is self-contained)
    [MaxLength(200)]
    public string? EmployeeName { get; set; }

    [MaxLength(200)]
    public string? Designation { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    public DateTime? ResignationDate { get; set; }

    public DateTime? LastWorkingDay { get; set; }

    [MaxLength(500)]
    public string? ReasonForLeaving { get; set; }

    // Audit
    public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}