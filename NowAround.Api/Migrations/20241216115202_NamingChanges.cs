using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NowAround.Api.Migrations
{
    /// <inheritdoc />
    public partial class NamingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryEstablishment_Categories_CategoriesId",
                table: "CategoryEstablishment");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryEstablishment_Establishments_EstablishmentsId",
                table: "CategoryEstablishment");

            migrationBuilder.DropForeignKey(
                name: "FK_CuisineEstablishment_Cuisines_CuisinesId",
                table: "CuisineEstablishment");

            migrationBuilder.DropForeignKey(
                name: "FK_CuisineEstablishment_Establishments_EstablishmentsId",
                table: "CuisineEstablishment");

            migrationBuilder.DropTable(
                name: "EstablishmentTag");

            migrationBuilder.RenameColumn(
                name: "EstablishmentsId",
                table: "CuisineEstablishment",
                newName: "EstablishmentId");

            migrationBuilder.RenameColumn(
                name: "CuisinesId",
                table: "CuisineEstablishment",
                newName: "CuisineId");

            migrationBuilder.RenameIndex(
                name: "IX_CuisineEstablishment_EstablishmentsId",
                table: "CuisineEstablishment",
                newName: "IX_CuisineEstablishment_EstablishmentId");

            migrationBuilder.RenameColumn(
                name: "EstablishmentsId",
                table: "CategoryEstablishment",
                newName: "EstablishmentId");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryEstablishment",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryEstablishment_EstablishmentsId",
                table: "CategoryEstablishment",
                newName: "IX_CategoryEstablishment_EstablishmentId");

            migrationBuilder.CreateTable(
                name: "TagEstablishment",
                columns: table => new
                {
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagEstablishment", x => new { x.EstablishmentId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagEstablishment_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagEstablishment_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagEstablishment_TagId",
                table: "TagEstablishment",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryEstablishment_Categories_CategoryId",
                table: "CategoryEstablishment",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryEstablishment_Establishments_EstablishmentId",
                table: "CategoryEstablishment",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CuisineEstablishment_Cuisines_CuisineId",
                table: "CuisineEstablishment",
                column: "CuisineId",
                principalTable: "Cuisines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CuisineEstablishment_Establishments_EstablishmentId",
                table: "CuisineEstablishment",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryEstablishment_Categories_CategoryId",
                table: "CategoryEstablishment");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryEstablishment_Establishments_EstablishmentId",
                table: "CategoryEstablishment");

            migrationBuilder.DropForeignKey(
                name: "FK_CuisineEstablishment_Cuisines_CuisineId",
                table: "CuisineEstablishment");

            migrationBuilder.DropForeignKey(
                name: "FK_CuisineEstablishment_Establishments_EstablishmentId",
                table: "CuisineEstablishment");

            migrationBuilder.DropTable(
                name: "TagEstablishment");

            migrationBuilder.RenameColumn(
                name: "EstablishmentId",
                table: "CuisineEstablishment",
                newName: "EstablishmentsId");

            migrationBuilder.RenameColumn(
                name: "CuisineId",
                table: "CuisineEstablishment",
                newName: "CuisinesId");

            migrationBuilder.RenameIndex(
                name: "IX_CuisineEstablishment_EstablishmentId",
                table: "CuisineEstablishment",
                newName: "IX_CuisineEstablishment_EstablishmentsId");

            migrationBuilder.RenameColumn(
                name: "EstablishmentId",
                table: "CategoryEstablishment",
                newName: "EstablishmentsId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "CategoryEstablishment",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryEstablishment_EstablishmentId",
                table: "CategoryEstablishment",
                newName: "IX_CategoryEstablishment_EstablishmentsId");

            migrationBuilder.CreateTable(
                name: "EstablishmentTag",
                columns: table => new
                {
                    EstablishmentsId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentTag", x => new { x.EstablishmentsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_EstablishmentTag_Establishments_EstablishmentsId",
                        column: x => x.EstablishmentsId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentTag_TagsId",
                table: "EstablishmentTag",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryEstablishment_Categories_CategoriesId",
                table: "CategoryEstablishment",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryEstablishment_Establishments_EstablishmentsId",
                table: "CategoryEstablishment",
                column: "EstablishmentsId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CuisineEstablishment_Cuisines_CuisinesId",
                table: "CuisineEstablishment",
                column: "CuisinesId",
                principalTable: "Cuisines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CuisineEstablishment_Establishments_EstablishmentsId",
                table: "CuisineEstablishment",
                column: "EstablishmentsId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
