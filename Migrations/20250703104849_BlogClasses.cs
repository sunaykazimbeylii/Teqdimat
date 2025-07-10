using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class BlogClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "BlogReviews");

            migrationBuilder.DropColumn(
                name: "UserImage",
                table: "BlogReviews");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "BlogReviews");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BlogReviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BlogReviews_UserId",
                table: "BlogReviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogReviews_AspNetUsers_UserId",
                table: "BlogReviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogReviews_AspNetUsers_UserId",
                table: "BlogReviews");

            migrationBuilder.DropIndex(
                name: "IX_BlogReviews_UserId",
                table: "BlogReviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BlogReviews");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BlogReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserImage",
                table: "BlogReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "BlogReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
