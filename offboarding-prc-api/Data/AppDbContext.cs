namespace offboarding_prc_api.Data;
using global::offboarding_prc_api.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // ── DbSets (one per table) ───────────────────────────────────
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<OffboardingRecord> OffboardingRecords { get; set; }
    public DbSet<StageData> StageDataList { get; set; }
    public DbSet<Clearance> Clearances { get; set; }
    public DbSet<AuditEntry> AuditLog { get; set; }

    // ── NEW ──────────────────────────────────────────────────────
    public DbSet<SubmissionLog> SubmissionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Indexes for common query patterns ────────────────────

        modelBuilder.Entity<StageData>()
            .HasIndex(s => new { s.RecordId, s.StageType });

        modelBuilder.Entity<Clearance>()
            .HasIndex(c => c.RecordId);

        modelBuilder.Entity<AuditEntry>()
            .HasIndex(a => new { a.RecordId, a.Timestamp });

        // SubmissionLog: query by EmployeeId or IsActive regularly
        modelBuilder.Entity<SubmissionLog>()
            .HasIndex(sl => sl.EmployeeId);

        modelBuilder.Entity<SubmissionLog>()
            .HasIndex(sl => sl.CreatedAt);

        // ── Relationships ────────────────────────────────────────

        modelBuilder.Entity<OffboardingRecord>()
            .HasOne(r => r.Employee)
            .WithMany(e => e.OffboardingRecords)
            .HasForeignKey(r => r.EmployeeId)
            .HasPrincipalKey(e => e.Id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<StageData>()
            .HasOne(s => s.Record)
            .WithMany(r => r.StageDataList)
            .HasForeignKey(s => s.RecordId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Clearance>()
            .HasOne(c => c.Record)
            .WithMany(r => r.Clearances)
            .HasForeignKey(c => c.RecordId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AuditEntry>()
            .HasOne(a => a.Record)
            .WithMany(r => r.AuditLog)
            .HasForeignKey(a => a.RecordId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Seed Data ────────────────────────────────────────────

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT" },
            new Department { Id = 2, Name = "Finance" },
            new Department { Id = 3, Name = "Admin" },
            new Department { Id = 4, Name = "HR" },
            new Department { Id = 5, Name = "Security" }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = "EMP001",
                Name = "Deepak Arjunan",
                Designation = "Software Engineer",
                Department = "Engineering",
                Email = "arjun.mehta@company.com",
                Manager = "Priya Sharma"
            },
            new Employee
            {
                Id = "EMP002",
                Name = "Deepak Kesavan",
                Designation = "Product Designer",
                Department = "Design",
                Email = "sneha.rao@company.com",
                Manager = "Rahul Nair"
            }
        );
    }
}