using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace offboarding_prc_api.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "off");

            migrationBuilder.RenameTable(
                name: "SubmissionLogs",
                newName: "SubmissionLogs",
                newSchema: "off");

            migrationBuilder.CreateTable(
                name: "ManagerApprovals",
                schema: "off",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ManagerEmpId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ManagerComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResignationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastWorkingDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReasonForLeaving = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerApprovals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagerApprovals_EmployeeId",
                schema: "off",
                table: "ManagerApprovals",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerApprovals_SubmissionLogId",
                schema: "off",
                table: "ManagerApprovals",
                column: "SubmissionLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerApprovals",
                schema: "off");

            migrationBuilder.RenameTable(
                name: "SubmissionLogs",
                schema: "off",
                newName: "SubmissionLogs");
        }
    }
}
