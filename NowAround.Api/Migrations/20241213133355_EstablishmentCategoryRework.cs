using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NowAround.Api.Migrations
{
    /// <inheritdoc />
    public partial class EstablishmentCategoryRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RatingStatistics_Establishments_EstablishmentId",
                table: "RatingStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Categories_CategoryId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "EstablishmentCategories");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CategoryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BusinessHoursId",
                table: "Establishments");

            migrationBuilder.DropColumn(
                name: "RatingCollectionId",
                table: "Establishments");

            migrationBuilder.CreateTable(
                name: "CategoryEstablishment",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    EstablishmentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryEstablishment", x => new { x.CategoriesId, x.EstablishmentsId });
                    table.ForeignKey(
                        name: "FK_CategoryEstablishment_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryEstablishment_Establishments_EstablishmentsId",
                        column: x => x.EstablishmentsId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEstablishment_EstablishmentsId",
                table: "CategoryEstablishment",
                column: "EstablishmentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_RatingStatistics_Establishments_EstablishmentId",
                table: "RatingStatistics",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RatingStatistics_Establishments_EstablishmentId",
                table: "RatingStatistics");

            migrationBuilder.DropTable(
                name: "CategoryEstablishment");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessHoursId",
                table: "Establishments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RatingCollectionId",
                table: "Establishments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstablishmentCategories",
                columns: table => new
                {
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentCategories", x => new { x.EstablishmentId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_EstablishmentCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentCategories_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CategoryId",
                table: "Tags",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentCategories_CategoryId",
                table: "EstablishmentCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentCategories_EstablishmentId",
                table: "EstablishmentCategories",
                column: "EstablishmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_RatingStatistics_Establishments_EstablishmentId",
                table: "RatingStatistics",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Categories_CategoryId",
                table: "Tags",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
