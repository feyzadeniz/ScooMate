using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public class IstatistikKayit
  {
    [Key]
    public int IstatistikID { get; set; }

    [Required]
    public DateTime Donem { get; set; }  // Ay ve yıl bilgisi

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AylikToplamHarcama { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GunlukOrtalamaHarcama { get; set; }

    // Kategorik harcama detayları (JSON olarak saklanacak)
    public string? KategorikHarcamaDetaylari { get; set; }

    // En yüksek harcamalar (JSON olarak saklanacak)
    public string? EnYuksekHarcamaDetaylari { get; set; }

    // Yıllık özet (JSON olarak saklanacak)
    public string? YillikOzetDetaylari { get; set; }

    [Required]
    public DateTime OlusturmaTarihi { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    [Required]
    [ForeignKey("Kullanici")]
    public int KullaniciID { get; set; }
    public virtual Kullanici? Kullanici { get; set; }
  }
}