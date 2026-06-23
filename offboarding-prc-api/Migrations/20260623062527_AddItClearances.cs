using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace offboarding_prc_api.Migrations
{
    /// <inheritdoc />
    public partial class AddItClearances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItClearances",
                schema: "off",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItEmpId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CorporateLaptopReturned = table.Column<bool>(type: "bit", nullable: false),
                    MobileDeviceReturned = table.Column<bool>(type: "bit", nullable: false),
                    SecurityBadgeReturned = table.Column<bool>(type: "bit", nullable: false),
                    AccessCardsReturned = table.Column<bool>(type: "bit", nullable: false),
                    CorporateEmailStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CloudInfraStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VpnAccessStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InternalToolsStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DeviceSerialNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SecondaryAssetNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClearedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItClearances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItClearances_EmployeeId",
                schema: "off",
                table: "ItClearances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItClearances_SubmissionLogId",
                schema: "off",
                table: "ItClearances",
                column: "SubmissionLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItClearances",
                schema: "off");
        }
    }
}
