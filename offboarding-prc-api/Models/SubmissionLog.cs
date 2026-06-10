namespace offboarding_prc_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ─────────────────────────────────────────────────────────────────
//  SUBMISSION LOG
//  Stores every form-submit action (e.g. "Resignation Submitted").
//  EmployeeData is stored as a JSON string (nvarchar(max)).
// ─────────────────────────────────────────────────────────────────
public class SubmissionLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // The employee this submission belongs to (stored as string to
    // match the existing Employee.Id column type).
    [Required, MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? PerformedBy { get; set; }

    // Arbitrary JSON blob — any shape the caller sends
    public string EmployeeData { get; set; } = "{}";

    [MaxLength(50)]
    public string? StageBefore { get; set; }

    [MaxLength(50)]
    public string? StageAfter { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}