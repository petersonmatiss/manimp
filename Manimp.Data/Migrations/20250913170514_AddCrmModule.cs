using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manimp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCrmModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bolts",
                columns: table => new
                {
                    BoltId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoltSpec = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Diameter = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Length = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Finish = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolts", x => x.BoltId);
                });

            migrationBuilder.CreateTable(
                name: "Coatings",
                columns: table => new
                {
                    CoatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CoatingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThicknessMicrons = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    CostPerSquareMeter = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coatings", x => x.CoatingId);
                    table.ForeignKey(
                        name: "FK_Coatings_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BillingAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultDeliveryAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    PartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: true),
                    ProfileTypeId = table.Column<int>(type: "int", nullable: true),
                    SteelGradeId = table.Column<int>(type: "int", nullable: true),
                    Dimensions = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Length = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    WeightPerPiece = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.PartId);
                    table.ForeignKey(
                        name: "FK_Parts_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "MaterialTypeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Parts_ProfileTypes_ProfileTypeId",
                        column: x => x.ProfileTypeId,
                        principalTable: "ProfileTypes",
                        principalColumn: "ProfileTypeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Parts_SteelGrades_SteelGradeId",
                        column: x => x.SteelGradeId,
                        principalTable: "SteelGrades",
                        principalColumn: "SteelGradeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_Contacts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmProjects",
                columns: table => new
                {
                    CrmProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeliveryRules = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectValue = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmProjects", x => x.CrmProjectId);
                    table.ForeignKey(
                        name: "FK_CrmProjects_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyLists",
                columns: table => new
                {
                    AssemblyListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrmProjectId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyLists", x => x.AssemblyListId);
                    table.ForeignKey(
                        name: "FK_AssemblyLists_CrmProjects_CrmProjectId",
                        column: x => x.CrmProjectId,
                        principalTable: "CrmProjects",
                        principalColumn: "CrmProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuttingLists",
                columns: table => new
                {
                    CuttingListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrmProjectId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GeneratedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingLists", x => x.CuttingListId);
                    table.ForeignKey(
                        name: "FK_CuttingLists_CrmProjects_CrmProjectId",
                        column: x => x.CrmProjectId,
                        principalTable: "CrmProjects",
                        principalColumn: "CrmProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    DeliveryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrmProjectId = table.Column<int>(type: "int", nullable: false),
                    DeliveryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PlannedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveredBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.DeliveryId);
                    table.ForeignKey(
                        name: "FK_Deliveries_CrmProjects_CrmProjectId",
                        column: x => x.CrmProjectId,
                        principalTable: "CrmProjects",
                        principalColumn: "CrmProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assemblies",
                columns: table => new
                {
                    AssemblyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyListId = table.Column<int>(type: "int", nullable: false),
                    AssemblyMark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ManufacturingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ManufacturingStarted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManufacturingCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assemblies", x => x.AssemblyId);
                    table.ForeignKey(
                        name: "FK_Assemblies_AssemblyLists_AssemblyListId",
                        column: x => x.AssemblyListId,
                        principalTable: "AssemblyLists",
                        principalColumn: "AssemblyListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackingLists",
                columns: table => new
                {
                    PackingListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryId = table.Column<int>(type: "int", nullable: true),
                    PackingListNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingLists", x => x.PackingListId);
                    table.ForeignKey(
                        name: "FK_PackingLists_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "DeliveryId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyCoatings",
                columns: table => new
                {
                    AssemblyCoatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    CoatingId = table.Column<int>(type: "int", nullable: false),
                    SurfaceArea = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyCoatings", x => x.AssemblyCoatingId);
                    table.ForeignKey(
                        name: "FK_AssemblyCoatings_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyCoatings_Coatings_CoatingId",
                        column: x => x.CoatingId,
                        principalTable: "Coatings",
                        principalColumn: "CoatingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyOutsourcings",
                columns: table => new
                {
                    AssemblyOutsourcingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyOutsourcings", x => x.AssemblyOutsourcingId);
                    table.ForeignKey(
                        name: "FK_AssemblyOutsourcings_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyOutsourcings_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyParts",
                columns: table => new
                {
                    AssemblyPartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    PartId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyParts", x => x.AssemblyPartId);
                    table.ForeignKey(
                        name: "FK_AssemblyParts_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyParts_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "PartId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuttingListEntries",
                columns: table => new
                {
                    CuttingListEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuttingListId = table.Column<int>(type: "int", nullable: false),
                    PartId = table.Column<int>(type: "int", nullable: false),
                    AssemblyId = table.Column<int>(type: "int", nullable: false),
                    SourceProfileInventoryId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CutLength = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    CutSequence = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuttingListEntries", x => x.CuttingListEntryId);
                    table.ForeignKey(
                        name: "FK_CuttingListEntries_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuttingListEntries_CuttingLists_CuttingListId",
                        column: x => x.CuttingListId,
                        principalTable: "CuttingLists",
                        principalColumn: "CuttingListId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuttingListEntries_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "PartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuttingListEntries_ProfileInventories_SourceProfileInventoryId",
                        column: x => x.SourceProfileInventoryId,
                        principalTable: "ProfileInventories",
                        principalColumn: "ProfileInventoryId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PackingListEntries",
                columns: table => new
                {
                    PackingListEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackingListId = table.Column<int>(type: "int", nullable: false),
                    AssemblyId = table.Column<int>(type: "int", nullable: true),
                    AssemblyOutsourcingId = table.Column<int>(type: "int", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Dimensions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingListEntries", x => x.PackingListEntryId);
                    table.ForeignKey(
                        name: "FK_PackingListEntries_Assemblies_AssemblyId",
                        column: x => x.AssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "AssemblyId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PackingListEntries_AssemblyOutsourcings_AssemblyOutsourcingId",
                        column: x => x.AssemblyOutsourcingId,
                        principalTable: "AssemblyOutsourcings",
                        principalColumn: "AssemblyOutsourcingId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PackingListEntries_PackingLists_PackingListId",
                        column: x => x.PackingListId,
                        principalTable: "PackingLists",
                        principalColumn: "PackingListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyPartBolts",
                columns: table => new
                {
                    AssemblyPartBoltId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssemblyPartId = table.Column<int>(type: "int", nullable: false),
                    BoltId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyPartBolts", x => x.AssemblyPartBoltId);
                    table.ForeignKey(
                        name: "FK_AssemblyPartBolts_AssemblyParts_AssemblyPartId",
                        column: x => x.AssemblyPartId,
                        principalTable: "AssemblyParts",
                        principalColumn: "AssemblyPartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssemblyPartBolts_Bolts_BoltId",
                        column: x => x.BoltId,
                        principalTable: "Bolts",
                        principalColumn: "BoltId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assemblies_AssemblyListId",
                table: "Assemblies",
                column: "AssemblyListId");

            migrationBuilder.CreateIndex(
                name: "IX_Assemblies_AssemblyMark",
                table: "Assemblies",
                column: "AssemblyMark");

            migrationBuilder.CreateIndex(
                name: "IX_Assemblies_ManufacturingCompleted",
                table: "Assemblies",
                column: "ManufacturingCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Assemblies_ManufacturingStarted",
                table: "Assemblies",
                column: "ManufacturingStarted");

            migrationBuilder.CreateIndex(
                name: "IX_Assemblies_ProgressPercentage",
                table: "Assemblies",
                column: "ProgressPercentage");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyCoatings_AppliedDate",
                table: "AssemblyCoatings",
                column: "AppliedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyCoatings_AssemblyId",
                table: "AssemblyCoatings",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyCoatings_CoatingId",
                table: "AssemblyCoatings",
                column: "CoatingId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyCoatings_Status",
                table: "AssemblyCoatings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyLists_CreatedUtc",
                table: "AssemblyLists",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyLists_CrmProjectId",
                table: "AssemblyLists",
                column: "CrmProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyOutsourcings_AssemblyId",
                table: "AssemblyOutsourcings",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyOutsourcings_ExpectedReturnDate",
                table: "AssemblyOutsourcings",
                column: "ExpectedReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyOutsourcings_SentDate",
                table: "AssemblyOutsourcings",
                column: "SentDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyOutsourcings_Status",
                table: "AssemblyOutsourcings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyOutsourcings_SupplierId",
                table: "AssemblyOutsourcings",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyPartBolts_AssemblyPartId",
                table: "AssemblyPartBolts",
                column: "AssemblyPartId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyPartBolts_BoltId",
                table: "AssemblyPartBolts",
                column: "BoltId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyParts_AssemblyId",
                table: "AssemblyParts",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyParts_AssemblyId_PartId",
                table: "AssemblyParts",
                columns: new[] { "AssemblyId", "PartId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyParts_PartId",
                table: "AssemblyParts",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_Bolts_BoltSpec",
                table: "Bolts",
                column: "BoltSpec",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bolts_Diameter_Length_Grade",
                table: "Bolts",
                columns: new[] { "Diameter", "Length", "Grade" });

            migrationBuilder.CreateIndex(
                name: "IX_Bolts_IsActive",
                table: "Bolts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Coatings_CoatingType",
                table: "Coatings",
                column: "CoatingType");

            migrationBuilder.CreateIndex(
                name: "IX_Coatings_IsActive",
                table: "Coatings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Coatings_Name",
                table: "Coatings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coatings_SupplierId",
                table: "Coatings",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CustomerId",
                table: "Contacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email",
                table: "Contacts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_IsActive",
                table: "Contacts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_IsPrimary",
                table: "Contacts",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_CrmProjects_CustomerId",
                table: "CrmProjects",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmProjects_IsActive",
                table: "CrmProjects",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CrmProjects_PlannedDeliveryDate",
                table: "CrmProjects",
                column: "PlannedDeliveryDate");

            migrationBuilder.CreateIndex(
                name: "IX_CrmProjects_ProjectCode",
                table: "CrmProjects",
                column: "ProjectCode",
                unique: true,
                filter: "[ProjectCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CrmProjects_StartDate",
                table: "CrmProjects",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_CrmProjects_Status",
                table: "CrmProjects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyName",
                table: "Customers",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerCode",
                table: "Customers",
                column: "CustomerCode",
                unique: true,
                filter: "[CustomerCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsActive",
                table: "Customers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingListEntries_AssemblyId",
                table: "CuttingListEntries",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingListEntries_CutSequence",
                table: "CuttingListEntries",
                column: "CutSequence");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingListEntries_CuttingListId",
                table: "CuttingListEntries",
                column: "CuttingListId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingListEntries_IsCompleted",
                table: "CuttingListEntries",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingListEntries_PartId",
                table: "CuttingListEntries",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingListEntries_SourceProfileInventoryId",
                table: "CuttingListEntries",
                column: "SourceProfileInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingLists_CrmProjectId",
                table: "CuttingLists",
                column: "CrmProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CuttingLists_GeneratedUtc",
                table: "CuttingLists",
                column: "GeneratedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_ActualDeliveryDate",
                table: "Deliveries",
                column: "ActualDeliveryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_CrmProjectId",
                table: "Deliveries",
                column: "CrmProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryNumber",
                table: "Deliveries",
                column: "DeliveryNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_PlannedDeliveryDate",
                table: "Deliveries",
                column: "PlannedDeliveryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_Status",
                table: "Deliveries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PackingListEntries_AssemblyId",
                table: "PackingListEntries",
                column: "AssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingListEntries_AssemblyOutsourcingId",
                table: "PackingListEntries",
                column: "AssemblyOutsourcingId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingListEntries_PackingListId",
                table: "PackingListEntries",
                column: "PackingListId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingLists_DeliveryId",
                table: "PackingLists",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingLists_PackingListNumber",
                table: "PackingLists",
                column: "PackingListNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackingLists_Purpose",
                table: "PackingLists",
                column: "Purpose");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_IsActive",
                table: "Parts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_MaterialTypeId_ProfileTypeId_SteelGradeId",
                table: "Parts",
                columns: new[] { "MaterialTypeId", "ProfileTypeId", "SteelGradeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartNumber",
                table: "Parts",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parts_ProfileTypeId",
                table: "Parts",
                column: "ProfileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_SteelGradeId",
                table: "Parts",
                column: "SteelGradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssemblyCoatings");

            migrationBuilder.DropTable(
                name: "AssemblyPartBolts");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "CuttingListEntries");

            migrationBuilder.DropTable(
                name: "PackingListEntries");

            migrationBuilder.DropTable(
                name: "Coatings");

            migrationBuilder.DropTable(
                name: "AssemblyParts");

            migrationBuilder.DropTable(
                name: "Bolts");

            migrationBuilder.DropTable(
                name: "CuttingLists");

            migrationBuilder.DropTable(
                name: "AssemblyOutsourcings");

            migrationBuilder.DropTable(
                name: "PackingLists");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "Assemblies");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "AssemblyLists");

            migrationBuilder.DropTable(
                name: "CrmProjects");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
