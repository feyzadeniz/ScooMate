
using Microsoft.EntityFrameworkCore;
using ScooMate.Models;
using System.Linq;

namespace ScooMate.Data
{
    public class ScoomateContext(DbContextOptions<ScoomateContext> options) : DbContext(options)
    {
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<Harcama> Harcamalar { get; set; }
        public DbSet<FaturaTakip> FaturaTakipler { get; set; }
        public DbSet<BorcTakip> BorcTakipler { get; set; }
        public DbSet<ButcePlanlama> ButcePlanlamalar { get; set; }
        public DbSet<ButceKalem> ButceKalemler { get; set; }
        public DbSet<IstatistikKayit> IstatistikKayitlari { get; set; }
        public DbSet<Yatirim> Yatirimlar { get; set; }
        public object? ButcePlanlamalari { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // tablo adlarını değiştirdim
            modelBuilder.Entity<Kullanici>().ToTable("KullanicilarNew");
            modelBuilder.Entity<Kategori>().ToTable("KategorilerNew");
            modelBuilder.Entity<Bildirim>().ToTable("BildirimlerNew");
            modelBuilder.Entity<Harcama>().ToTable("HarcamalarNew");
            modelBuilder.Entity<FaturaTakip>().ToTable("FaturaTakiplerNew");
            modelBuilder.Entity<BorcTakip>().ToTable("BorcTakiplerNew");
            modelBuilder.Entity<ButcePlanlama>().ToTable("ButcePlanlamalarNew");
            modelBuilder.Entity<ButceKalem>().ToTable("ButceKalemlerNew");
            modelBuilder.Entity<IstatistikKayit>().ToTable("IstatistikKayitlariNew");
            modelBuilder.Entity<Yatirim>().ToTable("YatirimlarNew");

            // harcama.Miktar için hassasiyet ayarı
            modelBuilder.Entity<Harcama>()
                .Property(h => h.Miktar)
                .HasPrecision(18, 2);

            // para alanları için hassasiyet ayarları
            modelBuilder.Entity<FaturaTakip>()
                .Property(f => f.Tutar)
                .HasPrecision(18, 2);
            modelBuilder.Entity<BorcTakip>()
                .Property(b => b.Tutar)
                .HasPrecision(18, 2);
            modelBuilder.Entity<BorcTakip>()
                .Property(b => b.OtomatikOdemeTutari)
                .HasPrecision(18, 2);

            // ButcePlanlama için hassasiyet ayarları
            modelBuilder.Entity<ButcePlanlama>()
                .Property(b => b.Gelirler)
                .HasPrecision(18, 2);
            modelBuilder.Entity<ButcePlanlama>()
                .Property(b => b.TemelGiderler)
                .HasPrecision(18, 2);
            modelBuilder.Entity<ButcePlanlama>()
                .Property(b => b.KullanilabilirLimit)
                .HasPrecision(18, 2);

            // ButceKalem için hassasiyet ayarları
            modelBuilder.Entity<ButceKalem>()
                .Property(b => b.Tutar)
                .HasPrecision(18, 2);

            // Default kategorileri ekle
            modelBuilder.Entity<Kategori>().HasData(
                new Kategori
                {
                    KategoriID = 1,
                    KategoriAdi = "Yemek",
                    Aciklama = "Yemek harcamaları",
                    KullaniciID = null  // default kategori
                },
                new Kategori
                {
                    KategoriID = 2,
                    KategoriAdi = "Ulaşım",
                    Aciklama = "Ulaşım harcamaları",
                    KullaniciID = null  // default kategori
                },
                new Kategori
                {
                    KategoriID = 3,
                    KategoriAdi = "Market",
                    Aciklama = "Market alışverişleri",
                    KullaniciID = null  // default kategori
                }
            );

            // harcama-Kategori ilişkisi
            modelBuilder.Entity<Harcama>()
                .HasOne(h => h.Kategori)
                .WithMany(k => k.Harcamalar)
                .HasForeignKey(h => h.KategoriID);
        }

        public void EnsureTablesCreated()
        {
            if (!Kategoriler.Any())
            {
                Kategoriler.AddRange(
                    new Kategori
                    {
                        KategoriAdi = "Yemek",
                        Aciklama = "Yemek harcamaları",
                        KullaniciID = null  // default kategori
                    },
                    new Kategori
                    {
                        KategoriAdi = "Ulaşım",
                        Aciklama = "Ulaşım harcamaları",
                        KullaniciID = null  // default kategori
                    },
                    new Kategori
                    {
                        KategoriAdi = "Market",
                        Aciklama = "Market alışverişleri",
                        KullaniciID = null  // default kategori
                    }
                );
                SaveChanges();
            }
        }
    }
}

