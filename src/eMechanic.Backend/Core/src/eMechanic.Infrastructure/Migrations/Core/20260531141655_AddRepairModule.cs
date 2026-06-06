using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eMechanic.Infrastructure.Migrations.Core
{
    /// <inheritdoc />
    public partial class AddRepairModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Repairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepairRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkshopId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstimatedCostAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EstimatedCostCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    FinalCostAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    FinalCostCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_RepairRequestId",
                table: "Repairs",
                column: "RepairRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_VehicleId",
                table: "Repairs",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_WorkshopId",
                table: "Repairs",
                column: "WorkshopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repairs");
        }
    }
}
