using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;

namespace ScooMate.Controllers
{
  public class KategoriController : Controller
  {
    private readonly ScoomateContext _db;

    public KategoriController(ScoomateContext db)
    {
      _db = db;
    }

    public IActionResult Detay(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var kategori = _db.Kategoriler
          .Include(k => k.Harcamalar.Where(h => h.KullaniciID == kullaniciId))
          .FirstOrDefault(k => k.KategoriID == id);

      if (kategori == null)
      {
        return NotFound();
      }

      // Debug için
      System.Diagnostics.Debug.WriteLine($"Kategori {id} için harcama sayısı: {kategori.Harcamalar.Count}");
      foreach (var harcama in kategori.Harcamalar)
      {
        System.Diagnostics.Debug.WriteLine($"Harcama: ID={harcama.HarcamaID}, Miktar={harcama.Miktar}, Tarih={harcama.Tarih}");
      }

      return View(kategori);
    }

    public IActionResult Ekle()
    {
      return View(new Kategori());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(Kategori kategori, IFormFile? ozelSimge)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
          kategori.KullaniciID = kullaniciId;

          if (ozelSimge != null && ozelSimge.Length > 0)
          {
            // dosya boyutu kontrolü (maksimum 2MB)
            if (ozelSimge.Length > 2 * 1024 * 1024)
            {
              ModelState.AddModelError("", "Simge dosyası 2MB'dan büyük olamaz.");
              return View(kategori);
            }

            var dosyaAdi = $"kategori-{Guid.NewGuid()}.png";
            var dosyaYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "kategoriler", dosyaAdi);
            var dizin = Path.GetDirectoryName(dosyaYolu);
            if (dizin != null)
            {
              Directory.CreateDirectory(dizin);
            }

            using (var stream = new FileStream(dosyaYolu, FileMode.Create))
            {
              await ozelSimge.CopyToAsync(stream);
            }

            kategori.OzelSimgeDosyasi = dosyaAdi;
          }

          _db.Kategoriler.Add(kategori);
          await _db.SaveChangesAsync();

          TempData["Mesaj"] = "Kategori başarıyla eklendi.";
          TempData["MesajTipi"] = "success";

          return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Kategori eklenirken bir hata oluştu.");
          System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
        }
      }

      return View(kategori);
    }

    public IActionResult Duzenle(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var kategori = _db.Kategoriler.FirstOrDefault(k => k.KategoriID == id &&
          (k.KullaniciID == kullaniciId || k.KullaniciID == null));
      if (kategori == null)
      {
        return NotFound();
      }
      return View(kategori);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, Kategori kategori, IFormFile? ozelSimge)
    {
      if (id != kategori.KategoriID)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          var mevcutKategori = await _db.Kategoriler.FindAsync(id);
          if (mevcutKategori == null)
          {
            return NotFound();
          }

          // yeni simge yüklendiyse
          if (ozelSimge != null && ozelSimge.Length > 0)
          {
            // dosya boyutu kontrolü (maksimum 2MB)
            if (ozelSimge.Length > 2 * 1024 * 1024)
            {
              ModelState.AddModelError("", "Simge dosyası 2MB'dan büyük olamaz.");
              return View(kategori);
            }

            // eski özel simge varsa sil
            if (!string.IsNullOrEmpty(mevcutKategori.OzelSimgeDosyasi))
            {
              var eskiDosyaYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "kategoriler", mevcutKategori.OzelSimgeDosyasi);
              if (System.IO.File.Exists(eskiDosyaYolu))
              {
                System.IO.File.Delete(eskiDosyaYolu);
              }
            }

            // yeni simgeyi kaydet
            var dosyaAdi = $"kategori-{Guid.NewGuid()}.png";
            var dosyaYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "kategoriler", dosyaAdi);
            Directory.CreateDirectory(Path.GetDirectoryName(dosyaYolu) ?? throw new InvalidOperationException("Dosya yolu oluşturulamadı"));

            using (var stream = new FileStream(dosyaYolu, FileMode.Create))
            {
              await ozelSimge.CopyToAsync(stream);
            }

            mevcutKategori.OzelSimgeDosyasi = dosyaAdi;
          }

          mevcutKategori.KategoriAdi = kategori.KategoriAdi;
          mevcutKategori.Aciklama = kategori.Aciklama;

          await _db.SaveChangesAsync();
          TempData["Mesaj"] = "Kategori başarıyla güncellendi.";
          TempData["MesajTipi"] = "success";

          return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Kategori güncellenirken bir hata oluştu.");
          System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
        }
      }

      return View(kategori);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var kategori = await _db.Kategoriler
          .Include(k => k.Harcamalar.Where(h => h.KullaniciID == kullaniciId))
          .FirstOrDefaultAsync(k => k.KategoriID == id &&
              (k.KullaniciID == kullaniciId || k.KullaniciID == null));

      if (kategori == null)
      {
        return NotFound();
      }

      try
      {
        // özel simge varsa sil
        if (!string.IsNullOrEmpty(kategori.OzelSimgeDosyasi))
        {
          var dosyaYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "kategoriler", kategori.OzelSimgeDosyasi);
          if (System.IO.File.Exists(dosyaYolu))
          {
            System.IO.File.Delete(dosyaYolu);
          }
        }

        // önce kategoriye ait harcamaları sil
        _db.Harcamalar.RemoveRange(kategori.Harcamalar);

        // sonra kategoriyi sil
        _db.Kategoriler.Remove(kategori);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Kategori ve ilişkili tüm harcamalar başarıyla silindi.";
        TempData["MesajTipi"] = "success";
      }
      catch (Exception ex)
      {
        TempData["Mesaj"] = "Kategori silinirken bir hata oluştu.";
        TempData["MesajTipi"] = "danger";
        System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
      }

      return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TumHarcamalariSil(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      // Kategoriyi ve ilişkili harcamaları getir
      var kategori = await _db.Kategoriler
          .Include(k => k.Harcamalar.Where(h => h.KullaniciID == kullaniciId))
          .FirstOrDefaultAsync(k => k.KategoriID == id);

      if (kategori == null)
      {
        return NotFound();
      }

      try
      {
        // Sadece bu kullanıcıya ait harcamaları sil
        var harcamalar = await _db.Harcamalar
            .Where(h => h.KategoriID == id && h.KullaniciID == kullaniciId)
            .ToListAsync();

        _db.Harcamalar.RemoveRange(harcamalar);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Tüm harcamalar başarıyla silindi.";
        TempData["MesajTipi"] = "success";
      }
      catch (Exception ex)
      {
        TempData["Mesaj"] = "Harcamalar silinirken bir hata oluştu.";
        TempData["MesajTipi"] = "danger";
        // Hata mesajını logla
        System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
      }

      return RedirectToAction(nameof(Detay), new { id = id });
    }
  }
}