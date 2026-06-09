namespace offboarding_prc_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ─────────────────────────────────────────────────────────────────
//  EMPLOYEE
//  Represents a company employee. In Phase 2 this will be populated
//  from the JWT / identity provider. For now it is seeded manually.
// ─────────────────────────────────────────────────────────────────
public class Employee
{
    [Key]
    [MaxLength(50)]   // must match OffboardingRecord.EmployeeId length exactly
    public string Id { get; set; } = string.Empty;          // e.g. "EMP001"

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Designation { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Manager { get; set; }

    // Navigation — one employee can have multiple offboarding records
    // (e.g. re-hired and exiting again)
    public ICollection<OffboardingRecord> OffboardingRecords { get; set; } = [];
}

// ─────────────────────────────────────────────────────────────────
//  DEPARTMENT
//  Master list of clearance departments.
// ─────────────────────────────────────────────────────────────────
public class Department
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Clearance> Clearances { get; set; } = [];
}

// ─────────────────────────────────────────────────────────────────
//  OFFBOARDING RECORD
//  One row per offboarding case. Holds the top-level employee info
//  and the current stage the case is in.
// ─────────────────────────────────────────────────────────────────
public class OffboardingRecord
{
    [Key]
    public int Id { get; set; }

    // Denormalised employee fields so the record is self-contained
    // even if the Employee master row is later updated.
    [Required, MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string EmployeeName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Designation { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(150)]
    public string? Manager { get; set; }

    // Exit details
    [MaxLength(100)]
    public string? ExitReason { get; set; }

    public string? ExitDetails { get; set; }

    public string? JobDescription { get; set; }

    public DateTime? EndDate { get; set; }

    public int NoticePeriod { get; set; } = 90;   // days

    // Stage tracking
    // Allowed values: exit_interview | manager_review | hr_initiation
    //                 clearances | final_approval | completed
    [Required, MaxLength(50)]
    public string CurrentStage { get; set; } = "exit_interview";

    [MaxLength(20)]
    public string Status { get; set; } = "active";  // active | completed

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Employee? Employee { get; set; }
    public ICollection<StageData> StageDataList { get; set; } = [];
    public ICollection<Clearance> Clearances { get; set; } = [];
    public ICollection<AuditEntry> AuditLog { get; set; } = [];
}

// ─────────────────────────────────────────────────────────────────
//  STAGE DATA
//  One row per stage per offboarding record. Stores the form payload
//  as JSON text. CompletedAt being non-null means the stage is done
//  and the form is locked on the frontend.
// ─────────────────────────────────────────────────────────────────
public class StageData
{
    [Key]
    public int Id { get; set; }

    public int RecordId { get; set; }

    // Matches the STAGES constants in the React store
    [Required, MaxLength(50)]
    public string StageType { get; set; } = string.Empty;

    // JSON blob — all form field values for this stage
    public string Payload { get; set; } = "{}";

    public DateTime? CompletedAt { get; set; }

    // Navigation
    [ForeignKey(nameof(RecordId))]
    public OffboardingRecord? Record { get; set; }
}

// ─────────────────────────────────────────────────────────────────
//  CLEARANCE
//  One row per department per offboarding record.
//  IsUnlocked is flipped to true by the T-2 day scheduled check
//  for the Finance department; all others start as true.
// ─────────────────────────────────────────────────────────────────
public class Clearance
{
    [Key]
    public int Id { get; set; }

    public int RecordId { get; set; }

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    // Finance starts false (locked until T-2); everyone else starts true
    public bool IsUnlocked { get; set; } = true;

    public bool IsCleared { get; set; } = false;

    [MaxLength(150)]
    public string? ClearedBy { get; set; }

    public DateTime? ClearedAt { get; set; }

    public string? Notes { get; set; }

    [ForeignKey(nameof(RecordId))]
    public OffboardingRecord? Record { get; set; }
}

// ─────────────────────────────────────────────────────────────────
//  AUDIT ENTRY
//  Immutable append-only log. Every stage transition and major
//  action writes a row here.
// ─────────────────────────────────────────────────────────────────
public class AuditEntry
{
    [Key]
    public int Id { get; set; }

    public int RecordId { get; set; }

    [Required, MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? PerformedBy { get; set; }

    [MaxLength(50)]
    public string? StageBefore { get; set; }

    [MaxLength(50)]
    public string? StageAfter { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(RecordId))]
    public OffboardingRecord? Record { get; set; }
}
