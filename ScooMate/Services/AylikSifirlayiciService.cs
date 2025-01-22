using Microsoft.EntityFrameworkCore;
using ScooMate.Data;

namespace ScooMate.Services
{
  public class AylikSifirlayiciService : BackgroundService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AylikSifirlayiciService> _logger;

    public AylikSifirlayiciService(
        IServiceProvider serviceProvider,
        ILogger<AylikSifirlayiciService> logger)
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

            // Tüm bildirimleri sıfırla
            await db.FaturaTakipler
                .ExecuteUpdateAsync(f => f
                    .SetProperty(x => x.BildirimGonderildi, false));

            await db.BorcTakipler
                .ExecuteUpdateAsync(b => b
                    .SetProperty(x => x.BildirimGonderildi, false));

            _logger.LogInformation("Aylık sıfırlama tamamlandı: {time}", DateTimeOffset.Now);
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Aylık sıfırlama sırasında hata oluştu");
        }

        // Bir sonraki ayın başını bekle
        var now = DateTime.Now;
        var nextMonth = now.AddMonths(1).Date.AddDays(1 - now.Day);
        var delay = nextMonth - now;

        await Task.Delay(delay, stoppingToken);
      }
    }
  }
}