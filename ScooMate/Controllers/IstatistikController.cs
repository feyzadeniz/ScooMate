using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;
using ScooMate.ViewModels;

namespace ScooMate.Controllers
{
  public class IstatistikController : Controller
  {
    private readonly ScoomateContext _db;

    public IstatistikController(ScoomateContext db)
    {
      _db = db;
    }

    public async Task<IActionResult> Index(DateTime? tarih = null)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var secilenTarih = tarih ?? DateTime.Now;
      var buAy = new DateTime(secilenTarih.Year, secilenTarih.Month, 1);
      var gelecekAy = buAy.AddMonths(1);

      var model = new IstatistikViewModel
      {
        Tarih = buAy,
        Donemler = await GetDonemler(kullaniciId.Value),

        // bu ayki toplam harcama
        AylikToplamHarcama = await _db.Harcamalar
            .Where(h => h.KullaniciID == kullaniciId &&
                   h.Tarih >= buAy && h.Tarih < gelecekAy)
            .SumAsync(h => h.Miktar),

        // kategorilere göre harcamalar
        KategorikHarcamalar = await _db.Harcamalar
            .Where(h => h.KullaniciID == kullaniciId &&
                   h.Tarih >= buAy && h.Tarih < gelecekAy &&
                   h.Kategori != null)
            .Include(h => h.Kategori)
            .GroupBy(h => h.Kategori!.KategoriAdi)
            .Select(g => new KategorikHarcama
            {
              Kategori = g.Key,
              ToplamTutar = g.Sum(h => h.Miktar),
              HarcamaSayisi = g.Count()
            })
            .OrderByDescending(k => k.ToplamTutar)
            .ToListAsync(),

        // en yüksek harcamalar
        EnYuksekHarcamalar = await _db.Harcamalar
            .Where(h => h.KullaniciID == kullaniciId &&
                   h.Tarih >= buAy && h.Tarih < gelecekAy &&
                   h.Kategori != null)
            .Include(h => h.Kategori)
            .OrderByDescending(h => h.Miktar)
            .Take(5)
            .Select(h => new EnYuksekHarcama
            {
              HarcamaKalemi = h.Kategori!.KategoriAdi,
              Tutar = h.Miktar,
              Tarih = h.Tarih,
              Aciklama = h.Aciklama
            })
            .ToListAsync()
      };

      // günlük ortalama harcama
      model.GunlukOrtalamaHarcama = await CalculateGunlukOrtalama(kullaniciId.Value, buAy, gelecekAy);

      // yıllık özeti hesapla
      var yilBasi = new DateTime(secilenTarih.Year, 1, 1);
      var yilSonu = yilBasi.AddYears(1);

      var yillikHarcamalar = await _db.Harcamalar
          .Where(h => h.KullaniciID == kullaniciId &&
                      h.Tarih >= yilBasi &&
                      h.Tarih < yilSonu &&
                      h.Kategori != null)
          .Include(h => h.Kategori)
          .ToListAsync();

      // ay ay harcamaları grupla
      var aylikHarcamalarYillik = yillikHarcamalar
          .GroupBy(h => new { h.Tarih.Year, h.Tarih.Month })
          .Select(g => new
          {
            Ay = new DateTime(g.Key.Year, g.Key.Month, 1),
            Tutar = g.Sum(h => h.Miktar)
          })
          .ToList();

      // kategorilerin harcamalarını grupla
      var kategorikHarcamalar = yillikHarcamalar
          .GroupBy(h => h.Kategori!.KategoriAdi)
          .Select(g => new
          {
            Kategori = g.Key,
            ToplamTutar = g.Sum(h => h.Miktar)
          })
          .OrderByDescending(k => k.ToplamTutar)
          .ToList();

      // yıllık özeti oluştur
      model.YillikOzet = new YillikOzet
      {
        Yil = secilenTarih.Year,
        YillikToplamHarcama = yillikHarcamalar.Sum(h => h.Miktar),

        // en yüksek ay bilgileri
        EnYuksekAylikHarcama = aylikHarcamalarYillik.Any() ?
              aylikHarcamalarYillik.Max(x => x.Tutar) : 0,
        EnYuksekAy = aylikHarcamalarYillik.Any() ?
              aylikHarcamalarYillik.OrderByDescending(x => x.Tutar).First().Ay.ToString("MMMM") : "-",

        // en düşük ay bilgileri
        EnDusukAylikHarcama = aylikHarcamalarYillik.Any() ?
              aylikHarcamalarYillik.Min(x => x.Tutar) : 0,
        EnDusukAy = aylikHarcamalarYillik.Any() ?
              aylikHarcamalarYillik.OrderBy(x => x.Tutar).First().Ay.ToString("MMMM") : "-",

        // aylık ortalama - sadece harcama olan ayları hesaba kat
        AylikOrtalama = aylikHarcamalarYillik.Any() ?
              yillikHarcamalar.Sum(h => h.Miktar) / aylikHarcamalarYillik.Count : 0,

        // en çok harcanan kategori bilgileri
        EnCokHarcananKategori = kategorikHarcamalar.FirstOrDefault()?.Kategori ?? "-",
        EnCokHarcananKategoriTutar = kategorikHarcamalar.FirstOrDefault()?.ToplamTutar ?? 0
      };

      return View(model);
    }

    private async Task<List<DateTime>> GetDonemler(int kullaniciId)
    {
      var donemler = await _db.Harcamalar
          .Where(h => h.KullaniciID == kullaniciId)
          .Select(h => new { h.Tarih.Year, h.Tarih.Month })
          .Distinct()
          .OrderByDescending(d => d.Year)
          .ThenByDescending(d => d.Month)
          .Select(d => new DateTime(d.Year, d.Month, 1))
          .ToListAsync();

      // eğer hiç dönem yoksa, şu anki ayı ekle
      if (!donemler.Any())
      {
        donemler.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
      }

      return donemler;
    }

    private async Task<decimal> CalculateGunlukOrtalama(int kullaniciId, DateTime buAy, DateTime gelecekAy)
    {
      // önce tüm harcamaları al
      var harcamalar = await _db.Harcamalar
          .Where(h => h.KullaniciID == kullaniciId &&
                 h.Tarih >= buAy && h.Tarih < gelecekAy)
          .ToListAsync();

      // harcama yoksa 0 döndür
      if (!harcamalar.Any())
      {
        return 0;
      }

      // toplam harcama
      var toplamHarcama = harcamalar.Sum(h => h.Miktar);

      // ay içindeki gün sayısı (bugün ayın sonundaysa tüm ay, değilse bugüne kadar)
      var sonGun = DateTime.Now.Month == buAy.Month ?
          DateTime.Now.Day :
          DateTime.DaysInMonth(buAy.Year, buAy.Month);

      // günlük ortalamayı hesapla
      return toplamHarcama / sonGun;
    }
  }
}