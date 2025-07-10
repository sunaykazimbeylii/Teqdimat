using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class TranslationLangUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TourTranslations_TourId",
                table: "TourTranslations");

            migrationBuilder.DropIndex(
                name: "IX_DestinationTranslations_DestinationId",
                table: "DestinationTranslations");

            migrationBuilder.DropIndex(
                name: "IX_DestinationCategoryTranslations_DestinationCategoryId",
                table: "DestinationCategoryTranslations");

            migrationBuilder.AlterColumn<string>(
                name: "LangCode",
                table: "TourTranslations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LangCode",
                table: "DestinationTranslations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LangCode",
                table: "DestinationCategoryTranslations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TourTranslations_TourId_LangCode",
                table: "TourTranslations",
                columns: new[] { "TourId", "LangCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DestinationTranslations_DestinationId_LangCode",
                table: "DestinationTranslations",
                columns: new[] { "DestinationId", "LangCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DestinationCategoryTranslations_DestinationCategoryId_LangCode",
                table: "DestinationCategoryTranslations",
                columns: new[] { "DestinationCategoryId", "LangCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TourTranslations_TourId_LangCode",
                table: "TourTranslations");

            migrationBuilder.DropIndex(
                name: "IX_DestinationTranslations_DestinationId_LangCode",
                table: "DestinationTranslations");

            migrationBuilder.DropIndex(
                name: "IX_DestinationCategoryTranslations_DestinationCategoryId_LangCode",
                table: "DestinationCategoryTranslations");

            migrationBuilder.AlterColumn<string>(
                name: "LangCode",
                table: "TourTranslations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LangCode",
                table: "DestinationTranslations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LangCode",
                table: "DestinationCategoryTranslations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_TourTranslations_TourId",
                table: "TourTranslations",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationTranslations_DestinationId",
                table: "DestinationTranslations",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationCategoryTranslations_DestinationCategoryId",
                table: "DestinationCategoryTranslations",
                column: "DestinationCategoryId");
        }
    }
}
