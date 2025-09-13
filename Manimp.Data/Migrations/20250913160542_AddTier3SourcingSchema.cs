using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manimp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTier3SourcingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfileUsageLogs_ProjectId",
                table: "ProfileUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_ProjectId",
                table: "ProfileInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_PurchaseOrderId",
                table: "ProfileInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_SupplierId",
                table: "ProfileInventories");

            migrationBuilder.AddColumn<int>(
                name: "PriceRequestLineId",
                table: "PurchaseOrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PriceRequests",
                columns: table => new
                {
                    PriceRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRequests", x => x.PriceRequestId);
                    table.ForeignKey(
                        name: "FK_PriceRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PriceRequests_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PriceQuotes",
                columns: table => new
                {
                    PriceQuoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuoteNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    PriceRequestId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceQuotes", x => x.PriceQuoteId);
                    table.ForeignKey(
                        name: "FK_PriceQuotes_PriceRequests_PriceRequestId",
                        column: x => x.PriceRequestId,
                        principalTable: "PriceRequests",
                        principalColumn: "PriceRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceQuotes_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceRequestLines",
                columns: table => new
                {
                    PriceRequestLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    PriceRequestId = table.Column<int>(type: "int", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: false),
                    ProfileTypeId = table.Column<int>(type: "int", nullable: false),
                    SteelGradeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRequestLines", x => x.PriceRequestLineId);
                    table.ForeignKey(
                        name: "FK_PriceRequestLines_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "MaterialTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceRequestLines_PriceRequests_PriceRequestId",
                        column: x => x.PriceRequestId,
                        principalTable: "PriceRequests",
                        principalColumn: "PriceRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceRequestLines_ProfileTypes_ProfileTypeId",
                        column: x => x.ProfileTypeId,
                        principalTable: "ProfileTypes",
                        principalColumn: "ProfileTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceRequestLines_SteelGrades_SteelGradeId",
                        column: x => x.SteelGradeId,
                        principalTable: "SteelGrades",
                        principalColumn: "SteelGradeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PriceRequestLineId",
                table: "PurchaseOrderLines",
                column: "PriceRequestLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUsageLogs_ProjectId_UsedDate",
                table: "ProfileUsageLogs",
                columns: new[] { "ProjectId", "UsedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUsageLogs_UsedBy",
                table: "ProfileUsageLogs",
                column: "UsedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_Location",
                table: "ProfileInventories",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_ProjectId_ReceivedDate",
                table: "ProfileInventories",
                columns: new[] { "ProjectId", "ReceivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_PurchaseOrderId_ReceivedDate",
                table: "ProfileInventories",
                columns: new[] { "PurchaseOrderId", "ReceivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_ReceivedDate",
                table: "ProfileInventories",
                column: "ReceivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_SupplierId_ReceivedDate",
                table: "ProfileInventories",
                columns: new[] { "SupplierId", "ReceivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_PriceRequestId_SupplierId",
                table: "PriceQuotes",
                columns: new[] { "PriceRequestId", "SupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_QuoteDate",
                table: "PriceQuotes",
                column: "QuoteDate");

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_QuoteNumber",
                table: "PriceQuotes",
                column: "QuoteNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_Status",
                table: "PriceQuotes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_SupplierId",
                table: "PriceQuotes",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequestLines_MaterialTypeId",
                table: "PriceRequestLines",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequestLines_PriceRequestId_LineNumber",
                table: "PriceRequestLines",
                columns: new[] { "PriceRequestId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequestLines_ProfileTypeId",
                table: "PriceRequestLines",
                column: "ProfileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequestLines_SteelGradeId",
                table: "PriceRequestLines",
                column: "SteelGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequests_ProjectId",
                table: "PriceRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequests_RequestDate",
                table: "PriceRequests",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequests_RequestNumber",
                table: "PriceRequests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequests_Status",
                table: "PriceRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRequests_SupplierId",
                table: "PriceRequests",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderLines_PriceRequestLines_PriceRequestLineId",
                table: "PurchaseOrderLines",
                column: "PriceRequestLineId",
                principalTable: "PriceRequestLines",
                principalColumn: "PriceRequestLineId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderLines_PriceRequestLines_PriceRequestLineId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "PriceQuotes");

            migrationBuilder.DropTable(
                name: "PriceRequestLines");

            migrationBuilder.DropTable(
                name: "PriceRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderLines_PriceRequestLineId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_ProfileUsageLogs_ProjectId_UsedDate",
                table: "ProfileUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ProfileUsageLogs_UsedBy",
                table: "ProfileUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_Location",
                table: "ProfileInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_ProjectId_ReceivedDate",
                table: "ProfileInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_PurchaseOrderId_ReceivedDate",
                table: "ProfileInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_ReceivedDate",
                table: "ProfileInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProfileInventories_SupplierId_ReceivedDate",
                table: "ProfileInventories");

            migrationBuilder.DropColumn(
                name: "PriceRequestLineId",
                table: "PurchaseOrderLines");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUsageLogs_ProjectId",
                table: "ProfileUsageLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_ProjectId",
                table: "ProfileInventories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_PurchaseOrderId",
                table: "ProfileInventories",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInventories_SupplierId",
                table: "ProfileInventories",
                column: "SupplierId");
        }
    }
}
