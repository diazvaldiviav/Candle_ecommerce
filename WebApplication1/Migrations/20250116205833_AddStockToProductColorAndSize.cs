using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Candle_API.Migrations
{
    /// <inheritdoc />
    public partial class AddStockToProductColorAndSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "ProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "ProductColors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "ProductSizes");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "ProductColors");
        }
    }
}
