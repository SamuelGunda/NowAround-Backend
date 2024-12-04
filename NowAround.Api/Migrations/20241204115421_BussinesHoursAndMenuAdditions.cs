using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NowAround.Api.Migrations
{
    /// <inheritdoc />
    public partial class BussinesHoursAndMenuAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SocialLinks_Establishments_EstablishmentId",
                table: "SocialLinks");

            migrationBuilder.AddColumn<int>(
                name: "BusinessHoursId",
                table: "Establishments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BusinessHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monday = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    Tuesday = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    Wednesday = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    Thursday = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    Friday = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    Saturday = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    Sunday = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessHours_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Price = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessHoursExceptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    BusinessHoursId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHoursExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessHoursExceptions_BusinessHours_BusinessHoursId",
                        column: x => x.BusinessHoursId,
                        principalTable: "BusinessHours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHours_EstablishmentId",
                table: "BusinessHours",
                column: "EstablishmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHoursExceptions_BusinessHoursId",
                table: "BusinessHoursExceptions",
                column: "BusinessHoursId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_EstablishmentId",
                table: "MenuItems",
                column: "EstablishmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialLinks_Establishments_EstablishmentId",
                table: "SocialLinks",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SocialLinks_Establishments_EstablishmentId",
                table: "SocialLinks");

            migrationBuilder.DropTable(
                name: "BusinessHoursExceptions");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "BusinessHours");

            migrationBuilder.DropColumn(
                name: "BusinessHoursId",
                table: "Establishments");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialLinks_Establishments_EstablishmentId",
                table: "SocialLinks",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id");
        }
    }
}
