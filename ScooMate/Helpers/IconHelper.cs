namespace ScooMate.Helpers
{
  public static class IconHelper
  {
    private static readonly Dictionary<string, string> KategoriSimgeleri = new()
        {
            { "yemek", "fas fa-utensils" },
            { "yeme", "fas fa-utensils" },
            { "içme", "fas fa-glass-cheers" },
            { "cafe", "fas fa-coffee" },
            { "kahve", "fas fa-coffee" },
            { "restoran", "fas fa-hamburger" },
            
            { "ulaşım", "fas fa-bus" },
            { "taksi", "fas fa-taxi" },
            { "metro", "fas fa-subway" },
            { "araç", "fas fa-car" },
            { "benzin", "fas fa-gas-pump" },
            
            { "eğlence", "fas fa-gamepad" },
            { "oyun", "fas fa-gamepad" },
            { "sinema", "fas fa-film" },
            { "tiyatro", "fas fa-theater-masks" },
            
            { "alışveriş", "fas fa-shopping-cart" },
            { "market", "fas fa-shopping-basket" },
            { "giyim", "fas fa-tshirt" },
            
            { "sağlık", "fas fa-heartbeat" },
            { "hastane", "fas fa-hospital" },
            { "ilaç", "fas fa-pills" },
            
            { "eğitim", "fas fa-graduation-cap" },
            { "okul", "fas fa-school" },
            { "kurs", "fas fa-book" },
            
            { "fatura", "fas fa-file-invoice-dollar" },
            { "kira", "fas fa-home" },
            { "aidat", "fas fa-building" },
            
            { "ev", "fas fa-home" },
            { "mobilya", "fas fa-couch" },
            { "tamirat", "fas fa-tools" },
            { "temizlik", "fas fa-broom" },
            { "bahçe", "fas fa-leaf" },
            
            { "telefon", "fas fa-mobile-alt" },
            { "bilgisayar", "fas fa-laptop" },
            { "internet", "fas fa-wifi" },
            { "elektronik", "fas fa-plug" },
            
            { "spor", "fas fa-running" },
            { "fitness", "fas fa-dumbbell" },
            { "vitamin", "fas fa-capsules" },
            { "eczane", "fas fa-prescription-bottle" },
            
            { "kuaför", "fas fa-cut" },
            { "bakım", "fas fa-spa" },
            { "kozmetik", "fas fa-pump-soap" },
            
            { "parti", "fas fa-glass-cheers" },
            { "konser", "fas fa-music" },
            { "müze", "fas fa-landmark" },
            { "seyahat", "fas fa-plane" },
            { "tatil", "fas fa-umbrella-beach" },
            
            { "banka", "fas fa-university" },
            { "kredi", "fas fa-credit-card" },
            { "sigorta", "fas fa-shield-alt" },
            { "yatırım", "fas fa-chart-line" },
            
            { "hediye", "fas fa-gift" },
            { "kitap", "fas fa-book" },
            { "hobi", "fas fa-palette" },
            { "hayvan", "fas fa-paw" },
            { "bebek", "fas fa-baby" },
            { "çocuk", "fas fa-child" }
        };

    public static string GetIconForCategory(string kategoriAdi)
    {
      // kategori adını küçük harfe çevir
      var normalizedAd = kategoriAdi.ToLower().Trim();

      // eşleşen anahtar kelime ara
      foreach (var pair in KategoriSimgeleri)
      {
        if (normalizedAd.Contains(pair.Key))
        {
          return pair.Value;
        }
      }

      // eşleşme bulunamazsa varsayılan simge
      return "fas fa-folder";
    }
  }
}