using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manimp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProjectTierField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectTier",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectTier",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectTier",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTier",
                table: "Projects",
                column: "ProjectTier");
        }
    }
}
