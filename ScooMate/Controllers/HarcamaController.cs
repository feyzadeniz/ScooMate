using Microsoft.AspNetCore.Mvc;
using ScooMate.Data;
using ScooMate.Models;
using Microsoft.EntityFrameworkCore;
using ScooMate.ViewModels;

namespace ScooMate.Controllers
{
  public class HarcamaController : Controller
  {
    private readonly ScoomateContext _db;

    public HarcamaController(ScoomateContext db)
    {
      _db = db;
    }

    public async Task<IActionResult> Ekle()
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      // URLden kategori IDsini al
      var kategoriId = Convert.ToInt32(Request.Query["kategoriId"]);
      var kategori = await _db.Kategoriler.FindAsync(kategoriId);

      if (kategori == null)
      {
        return NotFound();
      }

      // Eğer bu default kategori ise veya kullanıcının kendi kategorisi ise devam et
      if (kategori.KullaniciID != null && kategori.KullaniciID != kullaniciId)
      {
        return Unauthorized();
      }

      var harcama = new Harcama
      {
        KategoriID = kategoriId,
        Tarih = DateTime.Now,
        KullaniciID = kullaniciId.Value  // Kullanıcı ID'sini hemen ata
      };

      // Hem default kategorileri hem de kullanıcının kendi kategorilerini getir
      ViewBag.Kategoriler = await _db.Kategoriler
          .Where(k => k.KullaniciID == null || k.KullaniciID == kullaniciId)
          .ToListAsync();

      return View(harcama);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(Harcama harcama)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (!ModelState.IsValid || !kullaniciId.HasValue)
      {
        ViewBag.Kategoriler = await _db.Kategoriler
            .Where(k => k.KullaniciID == null || k.KullaniciID == kullaniciId)
            .ToListAsync();
        return View(harcama);
      }

      // Kategoriyi kontrol et
      var kategori = await _db.Kategoriler.FindAsync(harcama.KategoriID);
      if (kategori == null)
      {
        return NotFound();
      }

      // Eğer bu default kategori ise veya kullanıcının kendi kategorisi ise devam et
      if (kategori.KullaniciID != null && kategori.KullaniciID != kullaniciId)
      {
        return Unauthorized();
      }

      harcama.KullaniciID = kullaniciId.Value;
      _db.Harcamalar.Add(harcama);
      await _db.SaveChangesAsync();

      await IstatistikleriGuncelle(kullaniciId.Value, harcama.Tarih);

      TempData["Mesaj"] = "Harcama başarıyla eklendi.";
      TempData["MesajTipi"] = "success";

      return RedirectToAction("Detay", "Kategori", new { id = harcama.KategoriID });
    }

    public async Task<IActionResult> Duzenle(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var harcama = await _db.Harcamalar
          .Include(h => h.Kategori)
          .FirstOrDefaultAsync(h => h.HarcamaID == id && h.KullaniciID == kullaniciId);

      if (harcama == null)
      {
        return NotFound();
      }

      return View(harcama);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(Harcama harcama)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (!ModelState.IsValid || !kullaniciId.HasValue)
      {
        return View(harcama);
      }

      harcama.KullaniciID = kullaniciId.Value;
      _db.Harcamalar.Update(harcama);
      await _db.SaveChangesAsync();

      await IstatistikleriGuncelle(kullaniciId.Value, harcama.Tarih);

      TempData["Mesaj"] = "Harcama başarıyla güncellendi.";
      TempData["MesajTipi"] = "success";

      return RedirectToAction("Detay", "Kategori", new { id = harcama.KategoriID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (!kullaniciId.HasValue)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var harcama = await _db.Harcamalar
          .FirstOrDefaultAsync(h => h.HarcamaID == id && h.KullaniciID == kullaniciId);

      if (harcama == null)
      {
        return RedirectToAction("Index", "Home");
      }

      var kategoriId = harcama.KategoriID;
      var tarih = harcama.Tarih;

      _db.Harcamalar.Remove(harcama);
      await _db.SaveChangesAsync();

      // sstatistikleri güncelle
      await IstatistikleriGuncelle(kullaniciId.Value, tarih);

      TempData["Mesaj"] = "Harcama başarıyla silindi.";
      TempData["MesajTipi"] = "success";

      return RedirectToAction("Detay", "Kategori", new { id = kategoriId });
    }

    private async Task IstatistikleriGuncelle(int kullaniciId, DateTime tarih)
    {
      // istatistik kayıtlarını temizle ve yeniden hesapla
      var eskiKayitlar = await _db.IstatistikKayitlari
          .Where(i => i.KullaniciID == kullaniciId &&
                      i.Donem.Year == tarih.Year &&
                      i.Donem.Month == tarih.Month)
          .ToListAsync();

      if (eskiKayitlar.Any())
      {
        _db.IstatistikKayitlari.RemoveRange(eskiKayitlar);
        await _db.SaveChangesAsync();
      }

      // içinde olduğu ayın istatistik kaydını bul veya oluştur
      var buAy = new DateTime(tarih.Year, tarih.Month, 1);
      var istatistik = await _db.IstatistikKayitlari
          .FirstOrDefaultAsync(i => i.KullaniciID == kullaniciId && i.Donem == buAy);

      if (istatistik == null)
      {
        istatistik = new IstatistikKayit
        {
          KullaniciID = kullaniciId,
          Donem = buAy,
          OlusturmaTarihi = DateTime.Now
        };
        _db.IstatistikKayitlari.Add(istatistik);
      }

      // aylık toplam harcamayı hesapla
      var aylikHarcamalar = await _db.Harcamalar
          .Where(h => h.KullaniciID == kullaniciId &&
                     h.Tarih.Year == buAy.Year &&
                     h.Tarih.Month == buAy.Month)
          .Include(h => h.Kategori) // Kategori bilgisini dahil et
          .ToListAsync();

      istatistik.AylikToplamHarcama = aylikHarcamalar.Sum(h => h.Miktar);

      // günlük ortalama harcamayı hesapla
      var gunlukHarcamalar = aylikHarcamalar
          .GroupBy(h => h.Tarih.Date)
          .Select(g => g.Sum(h => h.Miktar));

      istatistik.GunlukOrtalamaHarcama = gunlukHarcamalar.Any()
          ? gunlukHarcamalar.Average()
          : 0;

      // kategorik harcamaları güncelle
      var kategorikHarcamalar = aylikHarcamalar
          .Where(h => h.Kategori != null)
          .GroupBy(h => h.Kategori!.KategoriAdi)
          .Select(g => new KategorikHarcama
          {
            Kategori = g.Key,
            ToplamTutar = g.Sum(h => h.Miktar),
            HarcamaSayisi = g.Count()
          })
          .OrderByDescending(k => k.ToplamTutar)
          .ToList();

      istatistik.KategorikHarcamaDetaylari =
          System.Text.Json.JsonSerializer.Serialize(kategorikHarcamalar);

      // en yüksek harcamaları güncelle
      var enYuksekHarcamalar = aylikHarcamalar
          .Where(h => h.Kategori != null)
          .OrderByDescending(h => h.Miktar)
          .Take(5)
          .Select(h => new EnYuksekHarcama
          {
            HarcamaKalemi = h.Kategori!.KategoriAdi,
            Tutar = h.Miktar,
            Tarih = h.Tarih,
            Aciklama = h.Aciklama
          })
          .ToList();

      istatistik.EnYuksekHarcamaDetaylari =
          System.Text.Json.JsonSerializer.Serialize(enYuksekHarcamalar);

      // yıllık özeti de güncelle
      var yillikOzet = await HesaplaYillikOzet(kullaniciId, tarih.Year);
      istatistik.YillikOzetDetaylari =
          System.Text.Json.JsonSerializer.Serialize(yillikOzet);

      istatistik.GuncellemeTarihi = DateTime.Now;

      await _db.SaveChangesAsync();
    }

    // yıllık özet hesaplama metodunu ayrı bir metod yap
    private async Task<YillikOzet> HesaplaYillikOzet(int kullaniciId, int yil)
    {
      var yilBasi = new DateTime(yil, 1, 1);
      var yilSonu = yilBasi.AddYears(1);

      var yillikHarcamalar = await _db.Harcamalar
          .Where(h => h.KullaniciID == kullaniciId &&
                      h.Tarih >= yilBasi &&
                      h.Tarih < yilSonu &&
                      h.Kategori != null)
          .Include(h => h.Kategori)
          .ToListAsync();

      var aylikHarcamalarYillik = yillikHarcamalar
          .GroupBy(h => new { h.Tarih.Year, h.Tarih.Month })
          .Select(g => new
          {
            Ay = new DateTime(g.Key.Year, g.Key.Month, 1),
            Tutar = g.Sum(h => h.Miktar)
          })
          .ToList();

      var enCokHarcananKategori = yillikHarcamalar
          .GroupBy(h => h.Kategori!.KategoriAdi)
          .OrderByDescending(g => g.Sum(h => h.Miktar))
          .FirstOrDefault();

      var yillikOzet = new YillikOzet
      {
        Yil = yil,
        YillikToplamHarcama = yillikHarcamalar.Sum(h => h.Miktar),
        EnYuksekAylikHarcama = aylikHarcamalarYillik.Any() ? aylikHarcamalarYillik.Max(x => x.Tutar) : 0,
        EnYuksekAy = aylikHarcamalarYillik.Any() ?
              aylikHarcamalarYillik.OrderByDescending(x => x.Tutar).First().Ay.ToString("MMMM") : "-",
        EnDusukAylikHarcama = aylikHarcamalarYillik.Any() ? aylikHarcamalarYillik.Min(x => x.Tutar) : 0,
        EnDusukAy = aylikHarcamalarYillik.Any() ?
              aylikHarcamalarYillik.OrderBy(x => x.Tutar).First().Ay.ToString("MMMM") : "-",
        AylikOrtalama = aylikHarcamalarYillik.Any() ? aylikHarcamalarYillik.Average(x => x.Tutar) : 0,
        EnCokHarcananKategori = enCokHarcananKategori?.Key ?? "-",
        EnCokHarcananKategoriTutar = enCokHarcananKategori?.Sum(h => h.Miktar) ?? 0
      };

      return yillikOzet;
    }
  }
}