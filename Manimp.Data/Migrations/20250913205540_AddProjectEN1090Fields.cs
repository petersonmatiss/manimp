using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manimp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectEN1090Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantProjectLimits");

            migrationBuilder.DropTable(
                name: "Tenant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DbName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SecretRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "TenantProjectLimits",
                columns: table => new
                {
                    TenantProjectLimitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddonProjects = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BaseLimit = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ProjectsCreated = table.Column<int>(type: "int", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProjectLimits", x => x.TenantProjectLimitId);
                    table.ForeignKey(
                        name: "FK_TenantProjectLimits_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
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
    }
}
