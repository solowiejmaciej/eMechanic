using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eMechanic.Infrastructure.Migrations.Core
{
    /// <inheritdoc />
    public partial class AddRepairRequestModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepairRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkshopId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    EstimatedCostAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    EstimatedCostCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairRequests_UserId",
                table: "RepairRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairRequests_VehicleId",
                table: "RepairRequests",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairRequests_WorkshopId",
                table: "RepairRequests",
                column: "WorkshopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepairRequests");
        }
    }
}
