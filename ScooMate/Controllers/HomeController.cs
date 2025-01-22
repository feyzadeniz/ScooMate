using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScooMate.Models;
using ScooMate.Data;
using ScooMate.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ScooMate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ScoomateContext _db;

        public HomeController(ScoomateContext db)
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

            var buAy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Önce harcamaları kullanıcıya göre filtrele, sonra kategorileri getir
            var kullaniciHarcamalari = _db.Harcamalar
                .Where(h => h.KullaniciID == kullaniciId)
                .ToList();

            var kategoriler = _db.Kategoriler
                .Where(k => k.KullaniciID == null || k.KullaniciID == kullaniciId)
                .ToList();

            // Her kategoriye sadece kullanıcının harcamalarını ata
            foreach (var kategori in kategoriler)
            {
                kategori.Harcamalar = kullaniciHarcamalari
                    .Where(h => h.KategoriID == kategori.KategoriID)
                    .ToList();
            }

            var viewModel = new AnasayfaViewModel
            {
                Kategoriler = kategoriler,
                Bildirimler = _db.Bildirimler
                    .Where(b => b.KullaniciID == kullaniciId)
                    .ToList(),
                BuAykiToplamHarcama = kullaniciHarcamalari
                    .Where(h => h.Tarih.Month == DateTime.Now.Month &&
                           h.Tarih.Year == DateTime.Now.Year)
                    .Sum(h => h.Miktar),
                AylikButce = _db.ButcePlanlamalar
                    .Where(b => b.KullaniciID == kullaniciId &&
                           b.Tarih.Month == buAy.Month &&
                           b.Tarih.Year == buAy.Year)
                    .FirstOrDefault()
            };

            return View(viewModel);
        }

        public IActionResult TestDB()
        {
            try
            {
                // veritabanı bağlantısını test et
                bool canConnect = _db.Database.CanConnect();

                // tsabloları kontrol et
                var kullaniciSayisi = _db.Kullanicilar.Count();
                var kategoriSayisi = _db.Kategoriler.Count();

                return Json(new
                {
                    connected = canConnect,
                    kullanicilar = kullaniciSayisi,
                    kategoriler = kategoriSayisi,
                    message = "Veritabanı bağlantısı başarılı"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
