using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eMechanic.Infrastructure.Migrations.Core
{
    /// <inheritdoc />
    public partial class RenameOwnerToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerUserId",
                table: "Vehicles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_OwnerUserId",
                table: "Vehicles",
                newName: "IX_Vehicles_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Vehicles",
                newName: "OwnerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_UserId",
                table: "Vehicles",
                newName: "IX_Vehicles_OwnerUserId");
        }
    }
}
