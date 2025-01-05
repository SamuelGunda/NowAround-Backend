using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NowAround.Api.Migrations
{
    /// <inheritdoc />
    public partial class Bugfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Establishments_EstablishmentId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_RatingStatistics_RatingCollectionId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Establishments_EstablishmentId",
                table: "Posts",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_RatingStatistics_RatingCollectionId",
                table: "Reviews",
                column: "RatingCollectionId",
                principalTable: "RatingStatistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Establishments_EstablishmentId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_RatingStatistics_RatingCollectionId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Establishments_EstablishmentId",
                table: "Posts",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_RatingStatistics_RatingCollectionId",
                table: "Reviews",
                column: "RatingCollectionId",
                principalTable: "RatingStatistics",
                principalColumn: "Id");
        }
    }
}
