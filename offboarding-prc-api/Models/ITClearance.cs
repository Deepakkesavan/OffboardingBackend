namespace offboarding_prc_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ─────────────────────────────────────────────────────────────────
//  IT CLEARANCE
//  Stored in the [off].[ItClearances] table.
//  Created when the IT department clicks "Approve IT Clearance" on
//  the IT Employee Details screen, after HR has already initiated.
//
//  Captures:
//    • Asset checklist (laptop, mobile, badge, access cards)
//    • Access revocation status (email, cloud, VPN, internal tools)
//    • Device serial number + secondary asset notes
//    • IT final comments
// ─────────────────────────────────────────────────────────────────
[Table("ItClearances", Schema = "off")]
public class ItClearance
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // The SubmissionLog row this clearance relates to
    [Required]
    public Guid SubmissionLogId { get; set; }

    // The employee being cleared
    [Required, MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    // The IT user who approved the clearance
    [Required, MaxLength(50)]
    public string ItEmpId { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ItName { get; set; }

    // ── Asset checklist ───────────────────────────────────────────
    public bool CorporateLaptopReturned { get; set; }
    public bool MobileDeviceReturned { get; set; }
    public bool SecurityBadgeReturned { get; set; }
    public bool AccessCardsReturned { get; set; }

    // ── Access revocation statuses ────────────────────────────────
    // Values: "suspended" | "revoked" | "pending"
    [MaxLength(20)]
    public string? CorporateEmailStatus { get; set; }

    [MaxLength(20)]
    public string? CloudInfraStatus { get; set; }

    [MaxLength(20)]
    public string? VpnAccessStatus { get; set; }

    [MaxLength(20)]
    public string? InternalToolsStatus { get; set; }

    // ── Asset verification ────────────────────────────────────────
    [MaxLength(200)]
    public string? DeviceSerialNumber { get; set; }

    public string? SecondaryAssetNotes { get; set; }

    // ── IT final comments ─────────────────────────────────────────
    public string? ItComments { get; set; }

    // ── Snapshot fields ───────────────────────────────────────────
    [MaxLength(200)]
    public string? EmployeeName { get; set; }

    [MaxLength(200)]
    public string? Designation { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    // Audit
    public DateTime ClearedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}