using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace offboarding_prc_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubmissionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EmployeeData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StageBefore = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StageAfter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionLogs_CreatedAt",
                table: "SubmissionLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionLogs_EmployeeId",
                table: "SubmissionLogs",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionLogs");
        }
    }
}
