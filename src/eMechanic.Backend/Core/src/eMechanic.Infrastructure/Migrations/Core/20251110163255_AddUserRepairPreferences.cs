using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eMechanic.Infrastructure.Migrations.Core
{
    /// <inheritdoc />
    public partial class AddUserRepairPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRepairPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartsPreference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TimelinePreference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRepairPreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRepairPreferences_UserId",
                table: "UserRepairPreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRepairPreferences");
        }
    }
}
