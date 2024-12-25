using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NowAround.Api.Migrations
{
    /// <inheritdoc />
    public partial class AccountImagesAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundPictureUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundPictureUrl",
                table: "Establishments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Establishments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundPictureUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BackgroundPictureUrl",
                table: "Establishments");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Establishments");
        }
    }
}
