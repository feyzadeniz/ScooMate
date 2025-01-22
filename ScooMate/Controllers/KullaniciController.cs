using Microsoft.AspNetCore.Mvc;
using ScooMate.Data;
using ScooMate.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScooMate.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly ScoomateContext _db;

        public KullaniciController(ScoomateContext db)
        {
            _db = db;
        }

        public ActionResult KayitOl()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KayitOl(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // email kontrolü
                    var mevcutKullanici = _db.Kullanicilar.FirstOrDefault(k => k.Email == kullanici.Email);
                    if (mevcutKullanici != null)
                    {
                        ModelState.AddModelError("", "Bu e-posta adresi zaten kayıtlı.");
                        return View(kullanici);
                    }

                    // şifreyi hashle
                    kullanici.Sifre = HashPassword(kullanici.Sifre);

                    // Kullanıcıyı kaydet
                    _db.Kullanicilar.Add(kullanici);
                    _db.SaveChanges();

                    return RedirectToAction("GirisYap");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}");
                }
            }
            return View(kullanici);
        }

        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GirisYap(string email, string sifre)
        {
            try
            {
                // Debug için
                System.Diagnostics.Debug.WriteLine($"Giriş denemesi - Email: {email}");

                // önce kullanıcıyı bul
                var kullanici = _db.Kullanicilar.FirstOrDefault(k => k.Email == email);

                if (kullanici == null)
                {
                    System.Diagnostics.Debug.WriteLine("Kullanıcı bulunamadı");
                    ModelState.AddModelError("", "Bu email adresi ile kayıtlı kullanıcı bulunamadı.");
                    return View();
                }

                System.Diagnostics.Debug.WriteLine("Kullanıcı bulundu, şifre kontrol ediliyor");

                // şifreyi kontrol et
                var hashedPassword = HashPassword(sifre);
                System.Diagnostics.Debug.WriteLine($"Girilen şifre hash: {hashedPassword}");
                System.Diagnostics.Debug.WriteLine($"DB'deki şifre hash: {kullanici.Sifre}");

                if (kullanici.Sifre != hashedPassword)
                {
                    System.Diagnostics.Debug.WriteLine("Şifre eşleşmedi");
                    ModelState.AddModelError("", "Hatalı şifre!");
                    return View();
                }

                // giriş başarılı
                System.Diagnostics.Debug.WriteLine("Giriş başarılı");
                HttpContext.Session.Clear();
                HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Giriş hatası: {ex.Message}");
                ModelState.AddModelError("", "Giriş yapılırken bir hata oluştu: " + ex.Message);
                return View();
            }
        }

        public ActionResult CikisYap()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("GirisYap");
        }

        public ActionResult Profil()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap");
            }

            var kullanici = _db.Kullanicilar.FirstOrDefault(k => k.KullaniciID == kullaniciId);
            if (kullanici != null)
            {
                return View(kullanici);
            }

            return RedirectToAction("GirisYap");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfilGuncelle(Kullanici model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap");
            }

            var kullanici = await _db.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                kullanici.Ad = model.Ad;
                kullanici.Soyad = model.Soyad;
                kullanici.Email = model.Email;
                kullanici.Telefon = model.Telefon;

                await _db.SaveChangesAsync();
                TempData["Mesaj"] = "Profil bilgileriniz başarıyla güncellendi.";
                TempData["MesajTipi"] = "success";
            }

            return RedirectToAction(nameof(Profil));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreDegistir(string mevcutSifre, string yeniSifre, string yeniSifreTekrar)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("GirisYap");
            }

            var kullanici = await _db.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici == null)
            {
                return NotFound();
            }

            if (yeniSifre != yeniSifreTekrar)
            {
                TempData["Mesaj"] = "Yeni şifreler eşleşmiyor.";
                TempData["MesajTipi"] = "danger";
                return RedirectToAction(nameof(Profil));
            }

            var hashedMevcutSifre = HashPassword(mevcutSifre);
            if (hashedMevcutSifre != kullanici.Sifre)
            {
                TempData["Mesaj"] = "Mevcut şifre hatalı.";
                TempData["MesajTipi"] = "danger";
                return RedirectToAction(nameof(Profil));
            }

            kullanici.Sifre = HashPassword(yeniSifre);
            await _db.SaveChangesAsync();

            TempData["Mesaj"] = "Şifreniz başarıyla değiştirildi.";
            TempData["MesajTipi"] = "success";

            return RedirectToAction(nameof(Profil));
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
