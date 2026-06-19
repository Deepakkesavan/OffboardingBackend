using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace offboarding_prc_api.Migrations
{
    /// <inheritdoc />
    public partial class AddHrInitiations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HrInitiations",
                schema: "off",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HrEmpId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HrName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HrComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastWorkingDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrInitiations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrInitiations_EmployeeId",
                schema: "off",
                table: "HrInitiations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrInitiations_SubmissionLogId",
                schema: "off",
                table: "HrInitiations",
                column: "SubmissionLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrInitiations",
                schema: "off");
        }
    }
}
