using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class reviewLang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LangCode",
                table: "ReviewTranslations");

            migrationBuilder.AddColumn<string>(
                name: "LangCode",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LangCode",
                table: "Reviews");

            migrationBuilder.AddColumn<string>(
                name: "LangCode",
                table: "ReviewTranslations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
