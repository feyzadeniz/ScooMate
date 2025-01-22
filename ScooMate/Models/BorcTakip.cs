using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public enum BorcTipi
  {
    [Display(Name = "Kredi")]
    Kredi,

    [Display(Name = "Kredi Kartı")]
    KrediKarti,

    [Display(Name = "Taksit")]
    Taksit,

    [Display(Name = "Vergi")]
    Vergi,

    [Display(Name = "Bireysel Borç")]
    BireyselBorc,

    [Display(Name = "Diğer")]
    Diger
  }

  public class BorcTakip
  {
    [Key]
    public int BorcID { get; set; }

    [Required(ErrorMessage = "Borç tipi zorunludur")]
    [Display(Name = "Borç Tipi")]
    public BorcTipi BorcTipi { get; set; }

    [Required(ErrorMessage = "Borç adı zorunludur")]
    [StringLength(100)]
    [Display(Name = "Borç Adı")]
    public string BorcAdi { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Aciklama { get; set; }

    [Required(ErrorMessage = "Borç tutarı zorunludur")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Borç Tutarı")]
    public decimal Tutar { get; set; }

    [Required(ErrorMessage = "Hesap kesim günü zorunludur")]
    [Display(Name = "Hesap Kesim Günü")]
    [Range(1, 31, ErrorMessage = "Gün 1-31 arasında olmalıdır")]
    public int HesapKesimGunu { get; set; }

    [Required(ErrorMessage = "Son ödeme günü zorunludur")]
    [Display(Name = "Son Ödeme Günü")]
    [Range(1, 31, ErrorMessage = "Gün 1-31 arasında olmalıdır")]
    public int SonOdemeGunu { get; set; }

    [Display(Name = "Otomatik Ödeme")]
    public bool OtomatikOdeme { get; set; }

    [Display(Name = "Otomatik Ödeme Tutarı")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? OtomatikOdemeTutari { get; set; }

    public bool BildirimGonderildi { get; set; }

    [Required]
    [ForeignKey("Kullanici")]
    public int KullaniciID { get; set; }
    public virtual Kullanici? Kullanici { get; set; }
  }
}