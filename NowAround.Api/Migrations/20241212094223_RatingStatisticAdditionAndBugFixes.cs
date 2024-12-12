using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NowAround.Api.Migrations
{
    /// <inheritdoc />
    public partial class RatingStatisticAdditionAndBugFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Establishments_EstablishmentId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "EstablishmentId",
                table: "Reviews",
                newName: "RatingCollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_EstablishmentId",
                table: "Reviews",
                newName: "IX_Reviews_RatingCollectionId");

            migrationBuilder.AddColumn<int>(
                name: "RatingCollectionId",
                table: "Establishments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BusinessHoursExceptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.CreateTable(
                name: "RatingStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OneStar = table.Column<int>(type: "int", nullable: false),
                    TwoStars = table.Column<int>(type: "int", nullable: false),
                    ThreeStars = table.Column<int>(type: "int", nullable: false),
                    FourStars = table.Column<int>(type: "int", nullable: false),
                    FiveStars = table.Column<int>(type: "int", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatingStatistics_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RatingStatistics_EstablishmentId",
                table: "RatingStatistics",
                column: "EstablishmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_RatingStatistics_RatingCollectionId",
                table: "Reviews",
                column: "RatingCollectionId",
                principalTable: "RatingStatistics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_RatingStatistics_RatingCollectionId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "RatingStatistics");

            migrationBuilder.DropColumn(
                name: "RatingCollectionId",
                table: "Establishments");

            migrationBuilder.RenameColumn(
                name: "RatingCollectionId",
                table: "Reviews",
                newName: "EstablishmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_RatingCollectionId",
                table: "Reviews",
                newName: "IX_Reviews_EstablishmentId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BusinessHoursExceptions",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Establishments_EstablishmentId",
                table: "Reviews",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
