using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manimp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTier2ProcurementAndRemnantsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PONumber",
                table: "ProfileInventories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ProfileInventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                table: "ProfileInventories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProfileRemnantInventories",
                columns: table => new
                {
                    ProfileRemnantInventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemnantLotNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RemainingLength = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    RemnantPieces = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    OriginalProfileInventoryId = table.Column<int>(type: "int", nullable: false),
                    ProfileUsageLogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileRemnantInventories", x => x.ProfileRemnantInventoryId);
                    table.ForeignKey(
                        name: "FK_ProfileRemnantInventories_ProfileInventories_OriginalProfileInventoryId",
                        column: x => x.OriginalProfileInventoryId,
                        principalTable: "ProfileInventories",
                        principalColumn: "ProfileInventoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileRemnantInventories_ProfileUsageLogs_ProfileUsageLogId",
                        column: x => x.ProfileUsageLogId,
                        principalTable: "ProfileUsageLogs",
                        principalColumn: "ProfileUsageLogId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PONumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.PurchaseOrderId);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderLines",
                columns: table => new
                {
                    PurchaseOrderLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    LineTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: false),
                    ProfileTypeId = table.Column<int>(type: "int", nullable: false),
                    SteelGradeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderLines", x => x.PurchaseOrderLineId);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "MaterialTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_ProfileTypes_ProfileTypeId",
                        column: x => x.ProfileTypeId,
                        principalTable: "ProfileTypes",
                        principalColumn: "ProfileTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_SteelGrades_SteelGradeId",
                        column: x => x.SteelGradeId,
                        principalTable: "SteelGrades",
                        principalColumn: "SteelGradeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_ProjectId",
                table: "ProfileInventories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_PurchaseOrderId",
                table: "ProfileInventories",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRemnantInventories_CreatedDate",
                table: "ProfileRemnantInventories",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRemnantInventories_IsAvailable",
                table: "ProfileRemnantInventories",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRemnantInventories_OriginalProfileInventoryId",
                table: "ProfileRemnantInventories",
                column: "OriginalProfileInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRemnantInventories_ProfileUsageLogId",
                table: "ProfileRemnantInventories",
                column: "ProfileUsageLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileRemnantInventories_RemnantLotNumber",
                table: "ProfileRemnantInventories",
                column: "RemnantLotNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_MaterialTypeId",
                table: "PurchaseOrderLines",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_ProfileTypeId",
                table: "PurchaseOrderLines",
                column: "ProfileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PurchaseOrderId_LineNumber",
                table: "PurchaseOrderLines",
                columns: new[] { "PurchaseOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_SteelGradeId",
                table: "PurchaseOrderLines",
                column: "SteelGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderDate",
                table: "PurchaseOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PONumber",
                table: "PurchaseOrders",
                column: "PONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileInventories_Projects_ProjectId",
                table: "ProfileInventories",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileInventories_PurchaseOrders_PurchaseOrderId",
                table: "ProfileInventories",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileInventories_Projects_ProjectId",
                table: "ProfileInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileInventories_PurchaseOrders_PurchaseOrderId",
                table: "ProfileInventories");

            migrationBuilder.DropTable(
                name: "ProfileRemnantInventories");

            migrationBuilder.DropTable(
                name: "PurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_ProjectId",
                table: "ProfileInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_PurchaseOrderId",
                table: "ProfileInventories");

            migrationBuilder.DropColumn(
                name: "PONumber",
                table: "ProfileInventories");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProfileInventories");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "ProfileInventories");
        }
    }
}
