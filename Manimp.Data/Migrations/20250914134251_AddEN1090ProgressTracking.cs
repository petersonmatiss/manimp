using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manimp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEN1090ProgressTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStatus",
                table: "Assemblies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCoatingOutsourced",
                table: "Assemblies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AssemblyStatusHistories",
                columns: table => new
                {
                    AssemblyStatusHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QACompleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyStatusHistories", x => x.AssemblyStatusHistoryId);
                    table.ForeignKey(
                        name: "FK_AssemblyStatusHistories_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonComplianceReports",
                columns: table => new
                {
                    NonComplianceReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NCRNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    DetectedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DetectedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RootCause = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImmediateActions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PreventiveActions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EN1090Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerNotificationRequired = table.Column<bool>(type: "bit", nullable: false),
                    CustomerNotifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonComplianceReports", x => x.NonComplianceReportId);
                    table.ForeignKey(
                        name: "FK_NonComplianceReports_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutsourcedCoatingTrackings",
                columns: table => new
                {
                    OutsourcedCoatingTrackingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CoatingSpecification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsourcedCoatingTrackings", x => x.OutsourcedCoatingTrackingId);
                    table.ForeignKey(
                        name: "FK_OutsourcedCoatingTrackings_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutsourcedCoatingTrackings_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QualityAssuranceRecords",
                columns: table => new
                {
                    QualityAssuranceRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    QAType = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    PerformedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ForStatus = table.Column<int>(type: "int", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EN1090Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityAssuranceRecords", x => x.QualityAssuranceRecordId);
                    table.ForeignKey(
                        name: "FK_QualityAssuranceRecords_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyStatusHistories_AssemblyId",
                table: "AssemblyStatusHistories",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyStatusHistories_ChangedBy",
                table: "AssemblyStatusHistories",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyStatusHistories_ChangedUtc",
                table: "AssemblyStatusHistories",
                column: "ChangedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyStatusHistories_NewStatus",
                table: "AssemblyStatusHistories",
                column: "NewStatus");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_AssemblyId",
                table: "NonComplianceReports",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_DetectedBy",
                table: "NonComplianceReports",
                column: "DetectedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_DetectedUtc",
                table: "NonComplianceReports",
                column: "DetectedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_NCRNumber",
                table: "NonComplianceReports",
                column: "NCRNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_ResponsiblePerson",
                table: "NonComplianceReports",
                column: "ResponsiblePerson");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_Severity",
                table: "NonComplianceReports",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_Status",
                table: "NonComplianceReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceReports_TargetResolutionDate",
                table: "NonComplianceReports",
                column: "TargetResolutionDate");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingTrackings_ActualReturnDate",
                table: "OutsourcedCoatingTrackings",
                column: "ActualReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingTrackings_AssemblyId",
                table: "OutsourcedCoatingTrackings",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingTrackings_ExpectedReturnDate",
                table: "OutsourcedCoatingTrackings",
                column: "ExpectedReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingTrackings_SentDate",
                table: "OutsourcedCoatingTrackings",
                column: "SentDate");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingTrackings_Status",
                table: "OutsourcedCoatingTrackings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingTrackings_SupplierId",
                table: "OutsourcedCoatingTrackings",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityAssuranceRecords_AssemblyId",
                table: "QualityAssuranceRecords",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityAssuranceRecords_ForStatus",
                table: "QualityAssuranceRecords",
                column: "ForStatus");

            migrationBuilder.CreateIndex(
                name: "IX_QualityAssuranceRecords_PerformedBy",
                table: "QualityAssuranceRecords",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QualityAssuranceRecords_PerformedUtc",
                table: "QualityAssuranceRecords",
                column: "PerformedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_QualityAssuranceRecords_QAType",
                table: "QualityAssuranceRecords",
                column: "QAType");

            migrationBuilder.CreateIndex(
                name: "IX_QualityAssuranceRecords_Result",
                table: "QualityAssuranceRecords",
                column: "Result");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssemblyStatusHistories");

            migrationBuilder.DropTable(
                name: "NonComplianceReports");

            migrationBuilder.DropTable(
                name: "OutsourcedCoatingTrackings");

            migrationBuilder.DropTable(
                name: "QualityAssuranceRecords");

            migrationBuilder.DropColumn(
                name: "CurrentStatus",
                table: "Assemblies");

            migrationBuilder.DropColumn(
                name: "IsCoatingOutsourced",
                table: "Assemblies");
        }
    }
}
