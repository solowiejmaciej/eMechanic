using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eMechanic.Infrastructure.Migrations.Core
{
    /// <inheritdoc />
    public partial class AddRepairRequestSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SummaryReport",
                table: "RepairRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SummaryReport",
                table: "RepairRequests");
        }
    }
}
