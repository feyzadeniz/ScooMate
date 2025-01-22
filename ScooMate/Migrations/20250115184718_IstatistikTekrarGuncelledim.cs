using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScooMate.Migrations
{
    /// <inheritdoc />
    public partial class IstatistikTekrarGuncelledim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IstatistikKayitlariNew",
                columns: table => new
                {
                    IstatistikID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Donem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AylikToplamHarcama = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GunlukOrtalamaHarcama = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KategorikHarcamaDetaylari = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnYuksekHarcamaDetaylari = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YillikOzetDetaylari = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IstatistikKayitlariNew", x => x.IstatistikID);
                    table.ForeignKey(
                        name: "FK_IstatistikKayitlariNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IstatistikKayitlariNew_KullaniciID",
                table: "IstatistikKayitlariNew",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IstatistikKayitlariNew");
        }
    }
}
