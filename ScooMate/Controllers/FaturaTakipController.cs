using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;
using System.Globalization;

namespace ScooMate.Controllers
{
  public class FaturaTakipController : Controller
  {
    private readonly ScoomateContext _db;

    public FaturaTakipController(ScoomateContext db)
    {
      _db = db;
    }

    public IActionResult Index()
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var faturalar = _db.FaturaTakipler
          .Where(f => f.KullaniciID == kullaniciId)
          .OrderBy(f => f.FaturaKesimGunu)
          .ToList();

      return View(faturalar);
    }

    public IActionResult Ekle()
    {
      return View(new FaturaTakip { Tutar = 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(FaturaTakip fatura)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      // tutar dönüşümü
      var tutarStr = Request.Form["Tutar"].ToString().Trim();
      if (!string.IsNullOrEmpty(tutarStr))
      {
        try
        {
          // virgülü noktaya çevir
          var normalizedTutar = tutarStr.Replace(",", ".");

          // InvariantCulture ile parse et
          if (decimal.TryParse(normalizedTutar,
              NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
              CultureInfo.InvariantCulture,
              out decimal tutar))
          {
            if (tutar <= 0)
            {
              ModelState.AddModelError("Tutar", "Tutar 0'dan büyük olmalıdır");
            }
            else
            {
              fatura.Tutar = tutar;
              ModelState.Remove("Tutar");
            }
          }
          else
          {
            ModelState.AddModelError("Tutar", "Geçerli bir tutar giriniz");
          }
        }
        catch
        {
          ModelState.AddModelError("Tutar", "Geçerli bir tutar giriniz");
        }
      }

      fatura.KullaniciID = kullaniciId.Value;
      fatura.BildirimGonderildi = false;

      if (ModelState.IsValid)
      {
        _db.FaturaTakipler.Add(fatura);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Fatura takibi başarıyla eklendi.";
        TempData["MesajTipi"] = "success";

        return RedirectToAction(nameof(Index));
      }

      return View(fatura);
    }

    // fatura bildirimlerini kontrol eden metod
    [HttpGet]
    public async Task<IActionResult> KontrolEtVeBildirimOlustur()
    {
      var bugun = DateTime.Now;
      var faturalar = await _db.FaturaTakipler
          .ToListAsync();

      foreach (var fatura in faturalar)
      {
        // fatura kesim tarihi yaklaştığında bildirim
        if (bugun.Day == fatura.FaturaKesimGunu)
        {
          var bildirim = new Bildirim
          {
            KullaniciID = fatura.KullaniciID,
            Mesaj = $"{fatura.FaturaAdi} için bugün fatura kesim günü!",
            OlusturulmaTarihi = DateTime.Now,
            Okundu = false
          };
          _db.Bildirimler.Add(bildirim);
          fatura.BildirimGonderildi = true;
        }

        // son ödeme tarihi yaklaştığında bildirim
        if (bugun.Day == fatura.SonOdemeGunu)
        {
          var bildirim = new Bildirim
          {
            KullaniciID = fatura.KullaniciID,
            Mesaj = $"{fatura.FaturaAdi} için bugün son ödeme günü!",
            OlusturulmaTarihi = DateTime.Now,
            Okundu = false
          };
          _db.Bildirimler.Add(bildirim);
          fatura.BildirimGonderildi = true;
        }
      }

      await _db.SaveChangesAsync();
      return Ok("Bildirimler kontrol edildi");
    }

    public IActionResult Duzenle(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var fatura = _db.FaturaTakipler
          .FirstOrDefault(f => f.FaturaID == id && f.KullaniciID == kullaniciId);

      if (fatura == null)
      {
        TempData["Mesaj"] = "Fatura bulunamadı.";
        TempData["MesajTipi"] = "danger";
        return RedirectToAction(nameof(Index));
      }

      return View(fatura);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(FaturaTakip fatura)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      // tutar dönüşümü (Ekle metodundaki ile aynı)
      var tutarStr = Request.Form["Tutar"].ToString().Trim();
      if (!string.IsNullOrEmpty(tutarStr))
      {
        try
        {
          var normalizedTutar = tutarStr.Replace(",", ".");
          if (decimal.TryParse(normalizedTutar,
              NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
              CultureInfo.InvariantCulture,
              out decimal tutar))
          {
            if (tutar <= 0)
            {
              ModelState.AddModelError("Tutar", "Tutar 0'dan büyük olmalıdır");
            }
            else
            {
              fatura.Tutar = tutar;
              ModelState.Remove("Tutar");
            }
          }
          else
          {
            ModelState.AddModelError("Tutar", "Geçerli bir tutar giriniz");
          }
        }
        catch
        {
          ModelState.AddModelError("Tutar", "Geçerli bir tutar giriniz");
        }
      }

      fatura.KullaniciID = kullaniciId.Value;

      if (ModelState.IsValid)
      {
        _db.FaturaTakipler.Update(fatura);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Fatura takibi başarıyla güncellendi.";
        TempData["MesajTipi"] = "success";

        return RedirectToAction(nameof(Index));
      }

      return View(fatura);
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

      var fatura = await _db.FaturaTakipler
          .FirstOrDefaultAsync(f => f.FaturaID == id && f.KullaniciID == kullaniciId);

      if (fatura != null)
      {
        _db.FaturaTakipler.Remove(fatura);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Fatura takibi başarıyla silindi.";
        TempData["MesajTipi"] = "success";
      }

      return RedirectToAction(nameof(Index));
    }
  }
}