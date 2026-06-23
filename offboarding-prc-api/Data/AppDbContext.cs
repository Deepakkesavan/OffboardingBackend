namespace offboarding_prc_api.Data;
using global::offboarding_prc_api.Models;
using Microsoft.EntityFrameworkCore;

// ─────────────────────────────────────────────────────────────────
//  APP DB CONTEXT
//
//  Active entities:
//    • SubmissionLog    — resignation submission tracking
//    • ManagerApproval  — manager approval records
//    • HrInitiation     — HR initiation records (post manager-approval)
//    • ItClearance      — IT department clearance records (post HR-initiation)
// ─────────────────────────────────────────────────────────────────
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SubmissionLog> SubmissionLogs { get; set; }
    public DbSet<ManagerApproval> ManagerApprovals { get; set; }
    public DbSet<HrInitiation> HrInitiations { get; set; }
    public DbSet<ItClearance> ItClearances { get; set; }

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
        modelBuilder.Entity<HrInitiation>()
            .HasIndex(hi => hi.SubmissionLogId);
        modelBuilder.Entity<HrInitiation>()
            .HasIndex(hi => hi.EmployeeId);
        modelBuilder.Entity<ItClearance>()
            .HasIndex(it => it.SubmissionLogId);
        modelBuilder.Entity<ItClearance>()
            .HasIndex(it => it.EmployeeId);

        // off schema tables
        modelBuilder.Entity<SubmissionLog>()
            .ToTable("SubmissionLogs", schema: "off");
        modelBuilder.Entity<ManagerApproval>()
            .ToTable("ManagerApprovals", schema: "off");
        modelBuilder.Entity<HrInitiation>()
            .ToTable("HrInitiations", schema: "off");
        modelBuilder.Entity<ItClearance>()
            .ToTable("ItClearances", schema: "off");
    }
}