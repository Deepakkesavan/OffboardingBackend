using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Model;

public partial class EmployeeDetailsContext : DbContext
{
    public EmployeeDetailsContext()
    {
    }

    public EmployeeDetailsContext(DbContextOptions<EmployeeDetailsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminApproval> AdminApprovals { get; set; }

    public virtual DbSet<EmployeeOffboard> EmployeeOffboards { get; set; }

    public virtual DbSet<HrApproval> HrApprovals { get; set; }

    public virtual DbSet<ItDepartment> ItDepartments { get; set; }

    public virtual DbSet<ReportingManagerApproval> ReportingManagerApprovals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=10.2.1.4,52752;Initial Catalog=employee-details;User ID=clarium-usr;Password=Clarium@123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminApproval>(entity =>
        {
            entity.HasKey(e => e.AdminId);

            entity.ToTable("AdminApproval");

            entity.Property(e => e.BusinessCards).HasMaxLength(100);
            entity.Property(e => e.CompanyBookorManuals).HasMaxLength(150);
            entity.Property(e => e.Companydocuments).HasMaxLength(150);
            entity.Property(e => e.IdentityCardorAccesscard).HasMaxLength(100);
            entity.Property(e => e.Laptopwithallaccessories).HasMaxLength(100);
            entity.Property(e => e.OfficeorDeskKeys).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeOffboard>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.ToTable("EmployeeOffboard");

            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(30)
                .HasColumnName("bankAccountNumber");
            entity.Property(e => e.ContactNumberResidence)
                .HasMaxLength(20)
                .HasColumnName("contactNumberResidence");
            entity.Property(e => e.DateOfJoining).HasColumnName("dateOfJoining");
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .HasColumnName("designation");
            entity.Property(e => e.EmployeeAddress)
                .HasMaxLength(250)
                .HasColumnName("employeeAddress");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .HasColumnName("employeeCode");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(150)
                .HasColumnName("employeeEmail");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(100)
                .HasColumnName("employeeName");
            entity.Property(e => e.EmploymentStatus)
                .HasMaxLength(50)
                .HasColumnName("employmentStatus");
            entity.Property(e => e.LastWorkingDay).HasColumnName("lastWorkingDay");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(20)
                .HasColumnName("mobileNumber");
            entity.Property(e => e.PanCardNumber)
                .HasMaxLength(20)
                .HasColumnName("panCardNumber");
            entity.Property(e => e.Project)
                .HasMaxLength(100)
                .HasColumnName("project");
            entity.Property(e => e.ResignationSubmittedDate).HasColumnName("resignationSubmittedDate");
        });

        modelBuilder.Entity<HrApproval>(entity =>
        {
            entity.HasKey(e => e.HrId);

            entity.ToTable("HrApproval");

            entity.Property(e => e.ExitInterviewFormAttached).HasMaxLength(100);
            entity.Property(e => e.IncomeTaxProofStatus).HasMaxLength(100);
            entity.Property(e => e.MediclaimOrMealCardStatus).HasMaxLength(100);
            entity.Property(e => e.NoticePayStatus).HasMaxLength(100);
            entity.Property(e => e.ResignationLetterAttached).HasMaxLength(100);
        });

        modelBuilder.Entity<ItDepartment>(entity =>
        {
            entity.ToTable("ItDepartment");

            entity.Property(e => e.BioMetricOdc)
                .HasMaxLength(100)
                .HasColumnName("BioMetricODC");
            entity.Property(e => e.Vdiaccess)
                .HasMaxLength(100)
                .HasColumnName("VDIAccess");
        });

        modelBuilder.Entity<ReportingManagerApproval>(entity =>
        {
            entity.HasKey(e => e.DocumentId);

            entity.ToTable("ReportingManagerApproval");

            entity.Property(e => e.DocumentId).HasColumnName("documentId");
            entity.Property(e => e.DocumentName)
                .HasMaxLength(200)
                .HasColumnName("documentName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
