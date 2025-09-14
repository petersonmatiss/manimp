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
            migrationBuilder.CreateTable(
                name: "OutsourcedCoatingLists",
                columns: table => new
                {
                    OutsourcedCoatingListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsourcedCoatingLists", x => x.OutsourcedCoatingListId);
                    table.ForeignKey(
                        name: "FK_OutsourcedCoatingLists_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyProgresses",
                columns: table => new
                {
                    AssemblyProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    CurrentStep = table.Column<int>(type: "int", nullable: false),
                    PreviousStep = table.Column<int>(type: "int", nullable: true),
                    CurrentStepStarted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStepCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsCoatingOutsourced = table.Column<bool>(type: "bit", nullable: false),
                    OutsourcedCoatingSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutsourcedCoatingExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutsourcedCoatingActualReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StepNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    OutsourcedCoatingListId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyProgresses", x => x.AssemblyProgressId);
                    table.ForeignKey(
                        name: "FK_AssemblyProgresses_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyProgresses_OutsourcedCoatingLists_OutsourcedCoatingListId",
                        column: x => x.OutsourcedCoatingListId,
                        principalTable: "OutsourcedCoatingLists",
                        principalColumn: "OutsourcedCoatingListId");
                });

            migrationBuilder.CreateTable(
                name: "AssemblyProgressStepHistories",
                columns: table => new
                {
                    AssemblyProgressStepHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyProgressId = table.Column<int>(type: "int", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: false),
                    StepStarted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StepCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DurationHours = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyProgressStepHistories", x => x.AssemblyProgressStepHistoryId);
                    table.ForeignKey(
                        name: "FK_AssemblyProgressStepHistories_AssemblyProgresses_AssemblyProgressId",
                        column: x => x.AssemblyProgressId,
                        principalTable: "AssemblyProgresses",
                        principalColumn: "AssemblyProgressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityChecks",
                columns: table => new
                {
                    QualityCheckId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyProgressId = table.Column<int>(type: "int", nullable: false),
                    CheckType = table.Column<int>(type: "int", nullable: false),
                    ForStep = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CheckedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckResults = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    StandardReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefectsFound = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityChecks", x => x.QualityCheckId);
                    table.ForeignKey(
                        name: "FK_QualityChecks_AssemblyProgresses_AssemblyProgressId",
                        column: x => x.AssemblyProgressId,
                        principalTable: "AssemblyProgresses",
                        principalColumn: "AssemblyProgressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonComplianceRecords",
                columns: table => new
                {
                    NonComplianceRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NCRNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QualityCheckId = table.Column<int>(type: "int", nullable: true),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    DiscoveredAtStep = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RootCause = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImmediateAction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PreventiveAction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DiscoveredBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscoveredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CustomerNotificationRequired = table.Column<bool>(type: "bit", nullable: false),
                    CustomerNotifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerResponse = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EN1090Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DocumentationReferences = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CostImpact = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    TimeImpactHours = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonComplianceRecords", x => x.NonComplianceRecordId);
                    table.ForeignKey(
                        name: "FK_NonComplianceRecords_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonComplianceRecords_QualityChecks_QualityCheckId",
                        column: x => x.QualityCheckId,
                        principalTable: "QualityChecks",
                        principalColumn: "QualityCheckId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgresses_AssemblyId",
                table: "AssemblyProgresses",
                column: "AssemblyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgresses_CurrentStep",
                table: "AssemblyProgresses",
                column: "CurrentStep");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgresses_CurrentStepStarted",
                table: "AssemblyProgresses",
                column: "CurrentStepStarted");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgresses_IsCoatingOutsourced",
                table: "AssemblyProgresses",
                column: "IsCoatingOutsourced");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgresses_OutsourcedCoatingListId",
                table: "AssemblyProgresses",
                column: "OutsourcedCoatingListId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgresses_UpdatedUtc",
                table: "AssemblyProgresses",
                column: "UpdatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgressStepHistories_AssemblyProgressId",
                table: "AssemblyProgressStepHistories",
                column: "AssemblyProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgressStepHistories_Step",
                table: "AssemblyProgressStepHistories",
                column: "Step");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgressStepHistories_StepCompleted",
                table: "AssemblyProgressStepHistories",
                column: "StepCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyProgressStepHistories_StepStarted",
                table: "AssemblyProgressStepHistories",
                column: "StepStarted");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_ActualResolutionDate",
                table: "NonComplianceRecords",
                column: "ActualResolutionDate");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_AssemblyId",
                table: "NonComplianceRecords",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_DiscoveredAtStep",
                table: "NonComplianceRecords",
                column: "DiscoveredAtStep");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_DiscoveredDate",
                table: "NonComplianceRecords",
                column: "DiscoveredDate");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_NCRNumber",
                table: "NonComplianceRecords",
                column: "NCRNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_QualityCheckId",
                table: "NonComplianceRecords",
                column: "QualityCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_Severity",
                table: "NonComplianceRecords",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_Status",
                table: "NonComplianceRecords",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NonComplianceRecords_TargetResolutionDate",
                table: "NonComplianceRecords",
                column: "TargetResolutionDate");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingLists_ExpectedReturnDate",
                table: "OutsourcedCoatingLists",
                column: "ExpectedReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingLists_SentDate",
                table: "OutsourcedCoatingLists",
                column: "SentDate");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingLists_Status",
                table: "OutsourcedCoatingLists",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutsourcedCoatingLists_SupplierId",
                table: "OutsourcedCoatingLists",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_AssemblyProgressId",
                table: "QualityChecks",
                column: "AssemblyProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_CheckedDate",
                table: "QualityChecks",
                column: "CheckedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_CheckType",
                table: "QualityChecks",
                column: "CheckType");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_ForStep",
                table: "QualityChecks",
                column: "ForStep");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_IsRequired",
                table: "QualityChecks",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_Status",
                table: "QualityChecks",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssemblyProgressStepHistories");

            migrationBuilder.DropTable(
                name: "NonComplianceRecords");

            migrationBuilder.DropTable(
                name: "QualityChecks");

            migrationBuilder.DropTable(
                name: "AssemblyProgresses");

            migrationBuilder.DropTable(
                name: "OutsourcedCoatingLists");
        }
    }
}
