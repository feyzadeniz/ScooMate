using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;
using System.Linq;

namespace ScooMate.Controllers
{
    public class YatirimController : Controller
    {
        private readonly ScoomateContext _db;

        public YatirimController(ScoomateContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            var yatirimlar = await _db.Yatirimlar
                .Where(y => y.KullaniciID == kullaniciId.Value)
                .OrderByDescending(y => y.Tarih)
                .ToListAsync();

            return View(yatirimlar);
        }

        public IActionResult Ekle()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            return View(new Yatirim { Tarih = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Yatirim yatirim)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            if (ModelState.IsValid)
            {
                yatirim.KullaniciID = kullaniciId.Value;
                _db.Yatirimlar.Add(yatirim);
                await _db.SaveChangesAsync();

                TempData["Mesaj"] = "Yatırım başarıyla eklendi.";
                TempData["MesajTipi"] = "success";

                return RedirectToAction(nameof(Index));
            }

            return View(yatirim);
        }

        public async Task<IActionResult> Duzenle(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            var yatirim = await _db.Yatirimlar
                .FirstOrDefaultAsync(y => y.YatirimID == id && y.KullaniciID == kullaniciId.Value);

            if (yatirim == null)
            {
                return NotFound();
            }

            return View(yatirim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(Yatirim yatirim)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            if (ModelState.IsValid)
            {
                yatirim.KullaniciID = kullaniciId.Value;
                _db.Yatirimlar.Update(yatirim);
                await _db.SaveChangesAsync();

                TempData["Mesaj"] = "Yatırım başarıyla güncellendi.";
                TempData["MesajTipi"] = "success";

                return RedirectToAction(nameof(Index));
            }

            return View(yatirim);
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

            var yatirim = await _db.Yatirimlar
                .FirstOrDefaultAsync(y => y.YatirimID == id && y.KullaniciID == kullaniciId.Value);

            if (yatirim != null)
            {
                _db.Yatirimlar.Remove(yatirim);
                await _db.SaveChangesAsync();

                TempData["Mesaj"] = "Yatırım başarıyla silindi.";
                TempData["MesajTipi"] = "success";
            }
            else
            {
                TempData["Mesaj"] = "Silme işlemi başarısız oldu.";
                TempData["MesajTipi"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
