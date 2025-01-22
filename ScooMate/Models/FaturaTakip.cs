using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public enum FaturaTipi
  {
    [Display(Name = "Elektrik Faturası")]
    Elektrik,

    [Display(Name = "Su Faturası")]
    Su,

    [Display(Name = "Doğalgaz Faturası")]
    Dogalgaz,

    [Display(Name = "Telefon Faturası")]
    Telefon,

    [Display(Name = "İnternet Faturası")]
    Internet,

    [Display(Name = "Kira")]
    Kira,

    [Display(Name = "Aidat")]
    Aidat,

    [Display(Name = "Diğer")]
    Diger
  }

  public class FaturaTakip
  {
    [Key]
    public int FaturaID { get; set; }

    [Required(ErrorMessage = "Fatura tipi zorunludur")]
    [Display(Name = "Fatura Tipi")]
    public FaturaTipi FaturaTipi { get; set; }

    [Required(ErrorMessage = "Fatura adı zorunludur")]
    [StringLength(100)]
    [Display(Name = "Fatura Adı")]
    public string FaturaAdi { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Aciklama { get; set; }

    [Required(ErrorMessage = "Tutar zorunludur")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır")]
    [RegularExpression(@"^\d{1,16}(,\d{0,2})?$", ErrorMessage = "Geçerli bir tutar giriniz")]
    [Display(Name = "Fatura Tutarı")]
    public decimal Tutar { get; set; }

    [Required(ErrorMessage = "Fatura kesim tarihi zorunludur")]
    [Display(Name = "Fatura Kesim Günü")]
    [Range(1, 31, ErrorMessage = "Gün 1-31 arasında olmalıdır")]
    public int FaturaKesimGunu { get; set; }

    [Required(ErrorMessage = "Son ödeme tarihi zorunludur")]
    [Display(Name = "Son Ödeme Günü")]
    [Range(1, 31, ErrorMessage = "Gün 1-31 arasında olmalıdır")]
    public int SonOdemeGunu { get; set; }

    [Display(Name = "Otomatik Ödeme")]
    public bool OtomatikOdeme { get; set; }

    public bool BildirimGonderildi { get; set; }

    [Required]
    [ForeignKey("Kullanici")]
    public int KullaniciID { get; set; }
    public virtual Kullanici? Kullanici { get; set; }
  }
}