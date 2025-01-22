using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;
using Microsoft.Extensions.Logging;

namespace ScooMate.Controllers
{
  public class ButcePlanlamaController : Controller
  {
    private readonly ScoomateContext _db;
    private readonly ILogger<ButcePlanlamaController> _logger;

    public ButcePlanlamaController(ScoomateContext db, ILogger<ButcePlanlamaController> logger)
    {
      _db = db;
      _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var buAy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
      var butcePlanlama = await _db.ButcePlanlamalar
          .Include(b => b.Kalemler)
          .Where(b => b.KullaniciID == kullaniciId &&
                 b.Tarih.Month == buAy.Month &&
                 b.Tarih.Year == buAy.Year)
          .FirstOrDefaultAsync();

      if (butcePlanlama == null)
      {
        butcePlanlama = new ButcePlanlama
        {
          KullaniciID = kullaniciId.Value,
          Tarih = buAy
        };
        _db.ButcePlanlamalar.Add(butcePlanlama);
        await _db.SaveChangesAsync();
      }

      return View(butcePlanlama);
    }

    [HttpPost]
    public async Task<IActionResult> KalemEkle(int butcePlanlamaId, string aciklama, string tutar, bool isGelir)
    {
      _logger.LogInformation($"KalemEkle: butcePlanlamaId={butcePlanlamaId}, aciklama={aciklama}, tutar={tutar}, isGelir={isGelir}");

      var butcePlanlama = await _db.ButcePlanlamalar
          .Include(b => b.Kalemler)
          .FirstOrDefaultAsync(b => b.ButcePlanlamaID == butcePlanlamaId);

      if (butcePlanlama == null)
      {
        return NotFound();
      }

      if (!decimal.TryParse(tutar.Replace(',', '.'),
          System.Globalization.NumberStyles.Number,
          System.Globalization.CultureInfo.InvariantCulture,
          out decimal tutarDecimal))
      {
        return BadRequest("Geçersiz tutar formatı");
      }

      var yeniKalem = new ButceKalem
      {
        Aciklama = aciklama,
        Tutar = tutarDecimal,
        IsGelir = isGelir,
        ButcePlanlamaID = butcePlanlamaId
      };

      butcePlanlama.Kalemler.Add(yeniKalem);
      butcePlanlama.LimitHesapla();
      await _db.SaveChangesAsync();

      _logger.LogInformation($"Yeni kalem eklendi: ID={yeniKalem.ButceKalemID}, Tutar={yeniKalem.Tutar}");

      return Json(new
      {
        success = true,
        kalemId = yeniKalem.ButceKalemID,
        yeniKalemTutar = yeniKalem.Tutar,
        yeniToplam = isGelir ? butcePlanlama.Gelirler : butcePlanlama.TemelGiderler,
        kullanilabilirLimit = butcePlanlama.KullanilabilirLimit
      });
    }

    [HttpPost]
    public async Task<IActionResult> KalemSil(int kalemId)
    {
      var kalem = await _db.ButceKalemler
          .Include(k => k.ButcePlanlama)
          .ThenInclude(b => b.Kalemler)
          .FirstOrDefaultAsync(k => k.ButceKalemID == kalemId);

      if (kalem == null)
      {
        return NotFound();
      }

      var butcePlanlama = kalem.ButcePlanlama;
      var isGelir = kalem.IsGelir;
      var silinecekTutar = kalem.Tutar;

      _db.ButceKalemler.Remove(kalem);
      await _db.SaveChangesAsync();

      butcePlanlama.LimitHesapla();

      if (isGelir && !butcePlanlama.Kalemler.Any(k => k.IsGelir))
      {
        butcePlanlama.Gelirler = 0;
      }
      else if (!isGelir && !butcePlanlama.Kalemler.Any(k => !k.IsGelir))
      {
        butcePlanlama.TemelGiderler = 0;
      }

      await _db.SaveChangesAsync();

      return Json(new
      {
        success = true,
        yeniToplam = isGelir ? butcePlanlama.Gelirler : butcePlanlama.TemelGiderler,
        kullanilabilirLimit = butcePlanlama.KullanilabilirLimit
      });
    }

    [HttpPost]
    public async Task<IActionResult> KalemDuzenle(int kalemId, string aciklama, decimal tutar)
    {
      var kalem = await _db.ButceKalemler
          .Include(k => k.ButcePlanlama)
          .ThenInclude(b => b.Kalemler)
          .FirstOrDefaultAsync(k => k.ButceKalemID == kalemId);

      if (kalem == null)
      {
        return NotFound();
      }

      kalem.Aciklama = aciklama;
      kalem.Tutar = tutar;

      var butcePlanlama = kalem.ButcePlanlama;
      butcePlanlama.LimitHesapla();
      await _db.SaveChangesAsync();

      return Json(new
      {
        success = true,
        yeniKalemTutar = kalem.Tutar,
        yeniToplam = kalem.IsGelir ? butcePlanlama.Gelirler : butcePlanlama.TemelGiderler,
        kullanilabilirLimit = butcePlanlama.KullanilabilirLimit
      });
    }

    [HttpPost]
    public async Task<IActionResult> LimitSifirla(int butcePlanlamaId)
    {
      var butcePlanlama = await _db.ButcePlanlamalar
          .Include(b => b.Kalemler)
          .FirstOrDefaultAsync(b => b.ButcePlanlamaID == butcePlanlamaId);

      if (butcePlanlama == null)
      {
        return NotFound();
      }

      _db.ButceKalemler.RemoveRange(butcePlanlama.Kalemler);
      butcePlanlama.Kalemler.Clear();
      butcePlanlama.KullanilabilirLimit = 0;
      butcePlanlama.LimitHesapla();
      await _db.SaveChangesAsync();

      return Json(new
      {
        success = true,
        gelirler = butcePlanlama.Gelirler,
        giderler = butcePlanlama.TemelGiderler
      });
    }
  }
}