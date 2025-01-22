using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScooMate.Controllers
{
  public class BildirimController : Controller
  {
    private readonly ScoomateContext _db;

    public BildirimController(ScoomateContext db)
    {
      _db = db;
    }

    public IActionResult Listele()
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null) return Json(new List<Bildirim>());

      var bildirimler = _db.Bildirimler
          .Where(b => b.KullaniciID == kullaniciId)
          .OrderByDescending(b => b.Yildizli)
          .ThenByDescending(b => b.OlusturulmaTarihi)
          .ToList();

      return Json(bildirimler);
    }

    [HttpPost]
    public IActionResult YildizGuncelle(int id)
    {
      var bildirim = _db.Bildirimler.Find(id);
      if (bildirim == null) return Json(new { success = false });

      bildirim.Yildizli = !bildirim.Yildizli;
      _db.SaveChanges();

      return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult Temizle()
    {
      var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
      if (kullaniciId == null) return Json(new { success = false });

      var bildirimler = _db.Bildirimler
          .Where(b => b.KullaniciID == kullaniciId && !b.Yildizli);
      _db.Bildirimler.RemoveRange(bildirimler);
      _db.SaveChanges();

      return Json(new { success = true });
    }
  }
}