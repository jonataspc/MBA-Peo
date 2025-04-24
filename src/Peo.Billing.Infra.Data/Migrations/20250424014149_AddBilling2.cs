using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.Billing.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBilling2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Payment",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Payment");
        }
    }
}
