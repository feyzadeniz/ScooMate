using Microsoft.EntityFrameworkCore;
using ScooMate.Data;
using ScooMate.Models;

namespace ScooMate.Services
{
  public class BildirimKontrolService : BackgroundService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BildirimKontrolService> _logger;

    public BildirimKontrolService(
        IServiceProvider serviceProvider,
        ILogger<BildirimKontrolService> logger)
    {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          using (var scope = _serviceProvider.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ScoomateContext>();
            var bugun = DateTime.Now;

            // Fatura bildirimleri
            var faturalar = await db.FaturaTakipler
                .Where(f => !f.BildirimGonderildi)
                .ToListAsync();

            foreach (var fatura in faturalar)
            {
              if (bugun.Day == fatura.FaturaKesimGunu)
              {
                var bildirim = new Bildirim
                {
                  KullaniciID = fatura.KullaniciID,
                  Mesaj = $"{fatura.FaturaAdi} için bugün fatura kesim günü!",
                  OlusturulmaTarihi = DateTime.Now,
                  Okundu = false
                };
                db.Bildirimler.Add(bildirim);
                fatura.BildirimGonderildi = true;
              }
              else if (bugun.Day == fatura.SonOdemeGunu)
              {
                var bildirim = new Bildirim
                {
                  KullaniciID = fatura.KullaniciID,
                  Mesaj = $"{fatura.FaturaAdi} için bugün son ödeme günü!",
                  OlusturulmaTarihi = DateTime.Now,
                  Okundu = false
                };
                db.Bildirimler.Add(bildirim);
                fatura.BildirimGonderildi = true;
              }
            }

            // Borç bildirimleri
            var borclar = await db.BorcTakipler
                .Where(b => !b.BildirimGonderildi)
                .ToListAsync();

            foreach (var borc in borclar)
            {
              if (bugun.Day == borc.HesapKesimGunu)
              {
                var bildirim = new Bildirim
                {
                  KullaniciID = borc.KullaniciID,
                  Mesaj = $"{borc.BorcAdi} için bugün hesap kesim günü!",
                  OlusturulmaTarihi = DateTime.Now,
                  Okundu = false
                };
                db.Bildirimler.Add(bildirim);
                borc.BildirimGonderildi = true;
              }
              else if (bugun.Day == borc.SonOdemeGunu)
              {
                var bildirim = new Bildirim
                {
                  KullaniciID = borc.KullaniciID,
                  Mesaj = $"{borc.BorcAdi} için bugün son ödeme günü!",
                  OlusturulmaTarihi = DateTime.Now,
                  Okundu = false
                };
                db.Bildirimler.Add(bildirim);
                borc.BildirimGonderildi = true;
              }
            }

            await db.SaveChangesAsync();
            _logger.LogInformation("Bildirim kontrolü tamamlandı: {time}", DateTimeOffset.Now);
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Bildirim kontrolü sırasında hata oluştu");
        }

        // Her saat başı kontrol et
        var now = DateTime.Now;
        var nextHour = now.AddHours(1).Date.AddHours(now.Hour + 1);
        var delay = nextHour - now;

        await Task.Delay(delay, stoppingToken);
      }
    }
  }
}