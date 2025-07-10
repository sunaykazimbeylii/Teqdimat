using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class newpropbooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "BookingTravellerTranslations");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "BookingTravellers",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "BookingTravellers");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "BookingTravellerTranslations",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }
    }
}
