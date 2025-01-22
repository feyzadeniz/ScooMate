using ScooMate.Models;
using System.Collections.Generic;

namespace ScooMate.Models.ViewModels
{
  public class AnasayfaViewModel
  {
    public IEnumerable<Kategori> Kategoriler { get; set; } = new List<Kategori>();
    public IEnumerable<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();
    public decimal BuAykiToplamHarcama { get; set; }
    public ButcePlanlama? AylikButce { get; set; }
    public IEnumerable<Yatirim> SonYatirimlar { get; set; } = new List<Yatirim>();
  }
}



