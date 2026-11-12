using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tooded_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Kassa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Toodetabel_Kategooriatabel_KategooriaId",
                table: "Toodetabel");

            migrationBuilder.AlterColumn<string>(
                name: "Pilt",
                table: "Toodetabel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Hind",
                table: "Toodetabel",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "Kirjeldus",
                table: "Kategooriatabel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Kasutajad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kasutajanimi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaisNimi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParoolHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sool = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roll = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Loodud = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kasutajad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kliendid",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Epost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kliendikaart = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Boonus = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Loodud = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kliendid", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Muugid",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kuupaev = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KasutajaId = table.Column<int>(type: "int", nullable: false),
                    KlientId = table.Column<int>(type: "int", nullable: true),
                    Vahesumma = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BoonusKasutatud = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Kokku = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BoonusSaadud = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PdfFail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muugid", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Muugid_Kasutajad_KasutajaId",
                        column: x => x.KasutajaId,
                        principalTable: "Kasutajad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Muugid_Kliendid_KlientId",
                        column: x => x.KlientId,
                        principalTable: "Kliendid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MuugiRead",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MuukId = table.Column<int>(type: "int", nullable: false),
                    ToodeId = table.Column<int>(type: "int", nullable: true),
                    Nimetus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kogus = table.Column<int>(type: "int", nullable: false),
                    Hind = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuugiRead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MuugiRead_Muugid_MuukId",
                        column: x => x.MuukId,
                        principalTable: "Muugid",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MuugiRead_Toodetabel_ToodeId",
                        column: x => x.ToodeId,
                        principalTable: "Toodetabel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kasutajad_Kasutajanimi",
                table: "Kasutajad",
                column: "Kasutajanimi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kliendid_Kliendikaart",
                table: "Kliendid",
                column: "Kliendikaart",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Muugid_KasutajaId",
                table: "Muugid",
                column: "KasutajaId");

            migrationBuilder.CreateIndex(
                name: "IX_Muugid_KlientId",
                table: "Muugid",
                column: "KlientId");

            migrationBuilder.CreateIndex(
                name: "IX_MuugiRead_MuukId",
                table: "MuugiRead",
                column: "MuukId");

            migrationBuilder.CreateIndex(
                name: "IX_MuugiRead_ToodeId",
                table: "MuugiRead",
                column: "ToodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Toodetabel_Kategooriatabel_KategooriaId",
                table: "Toodetabel",
                column: "KategooriaId",
                principalTable: "Kategooriatabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Toodetabel_Kategooriatabel_KategooriaId",
                table: "Toodetabel");

            migrationBuilder.DropTable(
                name: "MuugiRead");

            migrationBuilder.DropTable(
                name: "Muugid");

            migrationBuilder.DropTable(
                name: "Kasutajad");

            migrationBuilder.DropTable(
                name: "Kliendid");

            migrationBuilder.AlterColumn<string>(
                name: "Pilt",
                table: "Toodetabel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Hind",
                table: "Toodetabel",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Kirjeldus",
                table: "Kategooriatabel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Toodetabel_Kategooriatabel_KategooriaId",
                table: "Toodetabel",
                column: "KategooriaId",
                principalTable: "Kategooriatabel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
