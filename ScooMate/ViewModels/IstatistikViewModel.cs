namespace ScooMate.ViewModels
{
  public class IstatistikViewModel
  {
    public DateTime Tarih { get; set; }
    public string DonemAdi => $"{Tarih:MMMM yyyy}";
    public decimal AylikToplamHarcama { get; set; }
    public List<KategorikHarcama> KategorikHarcamalar { get; set; } = new();
    public List<EnYuksekHarcama> EnYuksekHarcamalar { get; set; } = new();
    public decimal GunlukOrtalamaHarcama { get; set; }
    public List<DateTime> Donemler { get; set; } = new();
    public YillikOzet YillikOzet { get; set; } = new();
  }

  public class KategorikHarcama
  {
    public string Kategori { get; set; } = string.Empty;
    public decimal ToplamTutar { get; set; }
    public int HarcamaSayisi { get; set; }
  }

  public class EnYuksekHarcama
  {
    public string HarcamaKalemi { get; set; } = string.Empty;
    public decimal Tutar { get; set; }
    public DateTime Tarih { get; set; }
    public string? Aciklama { get; set; }
  }

  public class YillikOzet
  {
    public YillikOzet()
    {
      EnYuksekAy = "-";
      EnDusukAy = "-";
      EnCokHarcananKategori = "-";
    }

    public int Yil { get; set; }
    public decimal YillikToplamHarcama { get; set; }
    public decimal EnYuksekAylikHarcama { get; set; }
    public string EnYuksekAy { get; set; } = string.Empty;
    public decimal EnDusukAylikHarcama { get; set; }
    public string EnDusukAy { get; set; } = string.Empty;
    public decimal AylikOrtalama { get; set; }
    public string EnCokHarcananKategori { get; set; } = string.Empty;
    public decimal EnCokHarcananKategoriTutar { get; set; }
  }
}