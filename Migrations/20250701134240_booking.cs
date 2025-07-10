using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class booking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdultsCount",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChildrenCount",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerAdult",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerChild",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PromoDiscountPercent",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdultsCount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ChildrenCount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PricePerAdult",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PricePerChild",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PromoDiscountPercent",
                table: "Bookings");
        }
    }
}
