using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NowAround.Api.Migrations
{
    /// <inheritdoc />
    public partial class EstablishmentTagsCuisineRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstablishmentCuisines");

            migrationBuilder.DropTable(
                name: "EstablishmentTags");

            migrationBuilder.CreateTable(
                name: "CuisineEstablishment",
                columns: table => new
                {
                    CuisinesId = table.Column<int>(type: "int", nullable: false),
                    EstablishmentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuisineEstablishment", x => new { x.CuisinesId, x.EstablishmentsId });
                    table.ForeignKey(
                        name: "FK_CuisineEstablishment_Cuisines_CuisinesId",
                        column: x => x.CuisinesId,
                        principalTable: "Cuisines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuisineEstablishment_Establishments_EstablishmentsId",
                        column: x => x.EstablishmentsId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_CuisineEstablishment_EstablishmentsId",
                table: "CuisineEstablishment",
                column: "EstablishmentsId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentTag_TagsId",
                table: "EstablishmentTag",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuisineEstablishment");

            migrationBuilder.DropTable(
                name: "EstablishmentTag");

            migrationBuilder.CreateTable(
                name: "EstablishmentCuisines",
                columns: table => new
                {
                    CuisineId = table.Column<int>(type: "int", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentCuisines", x => new { x.CuisineId, x.EstablishmentId });
                    table.ForeignKey(
                        name: "FK_EstablishmentCuisines_Cuisines_CuisineId",
                        column: x => x.CuisineId,
                        principalTable: "Cuisines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentCuisines_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentTags",
                columns: table => new
                {
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentTags", x => new { x.EstablishmentId, x.TagId });
                    table.ForeignKey(
                        name: "FK_EstablishmentTags_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentCuisines_CuisineId",
                table: "EstablishmentCuisines",
                column: "CuisineId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentCuisines_EstablishmentId",
                table: "EstablishmentCuisines",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentTags_EstablishmentId",
                table: "EstablishmentTags",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentTags_TagId",
                table: "EstablishmentTags",
                column: "TagId");
        }
    }
}
