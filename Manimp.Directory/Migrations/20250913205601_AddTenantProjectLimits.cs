using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manimp.Directory.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantProjectLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantProjectLimits",
                columns: table => new
                {
                    TenantProjectLimitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ProjectsCreated = table.Column<int>(type: "int", nullable: false),
                    BaseLimit = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    AddonProjects = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProjectLimits", x => x.TenantProjectLimitId);
                    table.ForeignKey(
                        name: "FK_TenantProjectLimits_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantProjectLimits_Month",
                table: "TenantProjectLimits",
                column: "Month");

            migrationBuilder.CreateIndex(
                name: "IX_TenantProjectLimits_TenantId",
                table: "TenantProjectLimits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantProjectLimits_TenantId_Month",
                table: "TenantProjectLimits",
                columns: new[] { "TenantId", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantProjectLimits");
        }
    }
}
