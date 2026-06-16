namespace offboarding_prc_api.Data;
using global::offboarding_prc_api.Models;
using Microsoft.EntityFrameworkCore;

// ─────────────────────────────────────────────────────────────────
//  APP DB CONTEXT
//
//  Trimmed to only the entities actually consumed by the frontend:
//    • SubmissionLog    — resignation submission tracking
//    • ManagerApproval  — manager approval records
//
//  Everything else (Employee, Department, OffboardingRecord,
//  StageData, Clearance, AuditEntry) was part of an earlier,
//  broader design that the current React app does not call into
//  and has been removed along with its controllers/services/DTOs.
// ─────────────────────────────────────────────────────────────────
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SubmissionLog> SubmissionLogs { get; set; }
    public DbSet<ManagerApproval> ManagerApprovals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SubmissionLog>()
            .HasIndex(sl => sl.EmployeeId);
        modelBuilder.Entity<SubmissionLog>()
            .HasIndex(sl => sl.CreatedAt);
        modelBuilder.Entity<ManagerApproval>()
            .HasIndex(ma => ma.SubmissionLogId);
        modelBuilder.Entity<ManagerApproval>()
            .HasIndex(ma => ma.EmployeeId);

        // off schema tables
        modelBuilder.Entity<SubmissionLog>()
            .ToTable("SubmissionLogs", schema: "off");
        modelBuilder.Entity<ManagerApproval>()
            .ToTable("ManagerApprovals", schema: "off");
    }
}