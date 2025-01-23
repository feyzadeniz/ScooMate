using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ScooMate.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KullanicilarNew",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullanicilarNew", x => x.KullaniciID);
                });

            migrationBuilder.CreateTable(
                name: "BildirimlerNew",
                columns: table => new
                {
                    BildirimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Okundu = table.Column<bool>(type: "bit", nullable: false),
                    Yildizli = table.Column<bool>(type: "bit", nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BildirimlerNew", x => x.BildirimID);
                    table.ForeignKey(
                        name: "FK_BildirimlerNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BorcTakiplerNew",
                columns: table => new
                {
                    BorcID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BorcTipi = table.Column<int>(type: "int", nullable: false),
                    BorcAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HesapKesimGunu = table.Column<int>(type: "int", nullable: false),
                    SonOdemeGunu = table.Column<int>(type: "int", nullable: false),
                    OtomatikOdeme = table.Column<bool>(type: "bit", nullable: false),
                    OtomatikOdemeTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BildirimGonderildi = table.Column<bool>(type: "bit", nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorcTakiplerNew", x => x.BorcID);
                    table.ForeignKey(
                        name: "FK_BorcTakiplerNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ButcePlanlamalarNew",
                columns: table => new
                {
                    ButcePlanlamaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gelirler = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TemelGiderler = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KullanilabilirLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButcePlanlamalarNew", x => x.ButcePlanlamaID);
                    table.ForeignKey(
                        name: "FK_ButcePlanlamalarNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaturaTakiplerNew",
                columns: table => new
                {
                    FaturaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaturaTipi = table.Column<int>(type: "int", nullable: false),
                    FaturaAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FaturaKesimGunu = table.Column<int>(type: "int", nullable: false),
                    SonOdemeGunu = table.Column<int>(type: "int", nullable: false),
                    OtomatikOdeme = table.Column<bool>(type: "bit", nullable: false),
                    BildirimGonderildi = table.Column<bool>(type: "bit", nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaturaTakiplerNew", x => x.FaturaID);
                    table.ForeignKey(
                        name: "FK_FaturaTakiplerNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "KategorilerNew",
                columns: table => new
                {
                    KategoriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OzelSimgeDosyasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciID = table.Column<int>(type: "int", nullable: true),
                    Simge = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KategorilerNew", x => x.KategoriID);
                    table.ForeignKey(
                        name: "FK_KategorilerNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID");
                });

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
                    Aciklama = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ButceKalemlerNew",
                columns: table => new
                {
                    ButceKalemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsGelir = table.Column<bool>(type: "bit", nullable: false),
                    ButcePlanlamaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButceKalemlerNew", x => x.ButceKalemID);
                    table.ForeignKey(
                        name: "FK_ButceKalemlerNew_ButcePlanlamalarNew_ButcePlanlamaID",
                        column: x => x.ButcePlanlamaID,
                        principalTable: "ButcePlanlamalarNew",
                        principalColumn: "ButcePlanlamaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HarcamalarNew",
                columns: table => new
                {
                    HarcamaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Miktar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KategoriID = table.Column<int>(type: "int", nullable: false),
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarcamalarNew", x => x.HarcamaID);
                    table.ForeignKey(
                        name: "FK_HarcamalarNew_KategorilerNew_KategoriID",
                        column: x => x.KategoriID,
                        principalTable: "KategorilerNew",
                        principalColumn: "KategoriID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HarcamalarNew_KullanicilarNew_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "KullanicilarNew",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "KategorilerNew",
                columns: new[] { "KategoriID", "Aciklama", "KategoriAdi", "KullaniciID", "OzelSimgeDosyasi", "Simge" },
                values: new object[,]
                {
                    { 1, "Yemek harcamaları", "Yemek", null, null, null },
                    { 2, "Ulaşım harcamaları", "Ulaşım", null, null, null },
                    { 3, "Market alışverişleri", "Market", null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BildirimlerNew_KullaniciID",
                table: "BildirimlerNew",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_BorcTakiplerNew_KullaniciID",
                table: "BorcTakiplerNew",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_ButceKalemlerNew_ButcePlanlamaID",
                table: "ButceKalemlerNew",
                column: "ButcePlanlamaID");

            migrationBuilder.CreateIndex(
                name: "IX_ButcePlanlamalarNew_KullaniciID",
                table: "ButcePlanlamalarNew",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_FaturaTakiplerNew_KullaniciID",
                table: "FaturaTakiplerNew",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_HarcamalarNew_KategoriID",
                table: "HarcamalarNew",
                column: "KategoriID");

            migrationBuilder.CreateIndex(
                name: "IX_HarcamalarNew_KullaniciID",
                table: "HarcamalarNew",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_IstatistikKayitlariNew_KullaniciID",
                table: "IstatistikKayitlariNew",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_KategorilerNew_KullaniciID",
                table: "KategorilerNew",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_YatirimlarNew_KullaniciID",
                table: "YatirimlarNew",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BildirimlerNew");

            migrationBuilder.DropTable(
                name: "BorcTakiplerNew");

            migrationBuilder.DropTable(
                name: "ButceKalemlerNew");

            migrationBuilder.DropTable(
                name: "FaturaTakiplerNew");

            migrationBuilder.DropTable(
                name: "HarcamalarNew");

            migrationBuilder.DropTable(
                name: "IstatistikKayitlariNew");

            migrationBuilder.DropTable(
                name: "YatirimlarNew");

            migrationBuilder.DropTable(
                name: "ButcePlanlamalarNew");

            migrationBuilder.DropTable(
                name: "KategorilerNew");

            migrationBuilder.DropTable(
                name: "KullanicilarNew");
        }
    }
}
