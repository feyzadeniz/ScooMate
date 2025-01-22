using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScooMate.Migrations
{
    /// <inheritdoc />
    public partial class YatirimEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YatirimlarNew",
                columns: table => new
                {
                    YatirimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YatirimlarNew", x => x.YatirimID);
                    table.ForeignKey(
                        name: "FK_YatirimlarNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YatirimlarNew_KullaniciID",
                table: "YatirimlarNew",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YatirimlarNew");
        }
    }
}
