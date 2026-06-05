using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eMechanic.Infrastructure.Migrations.Core
{
    /// <inheritdoc />
    public partial class AddPaymentOrderModule : Migration
    {
        private static readonly string[] PaymentOrderPendingLookupColumns =
        ["ReferenceId", "PayableType", "Status"];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderSessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CheckoutUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayableType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AmountValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_ReferenceId_PayableType_Status",
                table: "PaymentOrders",
                columns: PaymentOrderPendingLookupColumns);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_ProviderSessionId",
                table: "PaymentOrders",
                column: "ProviderSessionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentOrders");
        }
    }
}
