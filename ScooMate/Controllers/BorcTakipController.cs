using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;
using System.Globalization;

namespace ScooMate.Controllers
{
  public class BorcTakipController : Controller
  {
    private readonly ScoomateContext _db;

    public BorcTakipController(ScoomateContext db)
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

      var borclar = _db.BorcTakipler
          .Where(b => b.KullaniciID == kullaniciId)
          .OrderBy(b => b.SonOdemeGunu)
          .ToList();

      return View(borclar);
    }

    public IActionResult Ekle()
    {
      var model = new BorcTakip { Tutar = 0 };
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(BorcTakip borc)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      borc.KullaniciID = kullaniciId.Value;

      var tutarStr = Request.Form["Tutar"].ToString().Trim();
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
            borc.Tutar = tutar;
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

      if (borc.OtomatikOdeme)
      {
        var otomatikTutarStr = Request.Form["OtomatikOdemeTutari"].ToString().Trim();
        try
        {
          var normalizedTutar = otomatikTutarStr.Replace(",", ".");
          if (decimal.TryParse(normalizedTutar,
              NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
              CultureInfo.InvariantCulture,
              out decimal tutar))
          {
            if (tutar <= 0)
            {
              ModelState.AddModelError("OtomatikOdemeTutari", "Tutar 0'dan büyük olmalıdır");
            }
            else
            {
              borc.OtomatikOdemeTutari = tutar;
              ModelState.Remove("OtomatikOdemeTutari");
            }
          }
          else
          {
            ModelState.AddModelError("OtomatikOdemeTutari", "Geçerli bir tutar giriniz");
          }
        }
        catch
        {
          ModelState.AddModelError("OtomatikOdemeTutari", "Geçerli bir tutar giriniz");
        }
      }

      if (string.IsNullOrEmpty(borc.BorcAdi))
      {
        ModelState.AddModelError("BorcAdi", "Borç adı zorunludur");
      }
      if (borc.HesapKesimGunu < 1 || borc.HesapKesimGunu > 31)
      {
        ModelState.AddModelError("HesapKesimGunu", "Gün 1-31 arasında olmalıdır");
      }
      if (borc.SonOdemeGunu < 1 || borc.SonOdemeGunu > 31)
      {
        ModelState.AddModelError("SonOdemeGunu", "Gün 1-31 arasında olmalıdır");
      }

      if (ModelState.IsValid)
      {
        borc.BildirimGonderildi = false;
        _db.BorcTakipler.Add(borc);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Borç takibi başarıyla eklendi.";
        TempData["MesajTipi"] = "success";

        return RedirectToAction(nameof(Index));
      }

      return View(borc);
    }

    public IActionResult Duzenle(int id)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      var borc = _db.BorcTakipler
          .FirstOrDefault(b => b.BorcID == id && b.KullaniciID == kullaniciId);

      if (borc == null)
      {
        TempData["Mesaj"] = "Borç bulunamadı.";
        TempData["MesajTipi"] = "danger";
        return RedirectToAction(nameof(Index));
      }

      return View(borc);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(BorcTakip borc)
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null)
      {
        return RedirectToAction("GirisYap", "Kullanici");
      }

      borc.KullaniciID = kullaniciId.Value;

      var tutarStr = Request.Form["Tutar"].ToString().Trim();
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
            borc.Tutar = tutar;
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

      if (borc.OtomatikOdeme)
      {
        var otomatikTutarStr = Request.Form["OtomatikOdemeTutari"].ToString().Trim();
        try
        {
          var normalizedTutar = otomatikTutarStr.Replace(",", ".");
          if (decimal.TryParse(normalizedTutar,
              NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
              CultureInfo.InvariantCulture,
              out decimal tutar))
          {
            if (tutar <= 0)
            {
              ModelState.AddModelError("OtomatikOdemeTutari", "Tutar 0'dan büyük olmalıdır");
            }
            else
            {
              borc.OtomatikOdemeTutari = tutar;
              ModelState.Remove("OtomatikOdemeTutari");
            }
          }
          else
          {
            ModelState.AddModelError("OtomatikOdemeTutari", "Geçerli bir tutar giriniz");
          }
        }
        catch
        {
          ModelState.AddModelError("OtomatikOdemeTutari", "Geçerli bir tutar giriniz");
        }
      }

      if (string.IsNullOrEmpty(borc.BorcAdi))
      {
        ModelState.AddModelError("BorcAdi", "Borç adı zorunludur");
      }
      if (borc.HesapKesimGunu < 1 || borc.HesapKesimGunu > 31)
      {
        ModelState.AddModelError("HesapKesimGunu", "Gün 1-31 arasında olmalıdır");
      }
      if (borc.SonOdemeGunu < 1 || borc.SonOdemeGunu > 31)
      {
        ModelState.AddModelError("SonOdemeGunu", "Gün 1-31 arasında olmalıdır");
      }

      if (ModelState.IsValid)
      {
        _db.BorcTakipler.Update(borc);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Borç takibi başarıyla güncellendi.";
        TempData["MesajTipi"] = "success";

        return RedirectToAction(nameof(Index));
      }

      return View(borc);
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

      var borc = await _db.BorcTakipler
          .FirstOrDefaultAsync(b => b.BorcID == id && b.KullaniciID == kullaniciId);

      if (borc != null)
      {
        _db.BorcTakipler.Remove(borc);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Borç takibi başarıyla silindi.";
        TempData["MesajTipi"] = "success";
      }

      return RedirectToAction(nameof(Index));
    }
  }
}