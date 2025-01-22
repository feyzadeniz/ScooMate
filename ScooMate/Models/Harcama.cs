using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public class Harcama
  {
    [Key]
    public int HarcamaID { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Miktar { get; set; }

    [Required]
    public DateTime Tarih { get; set; }

    [StringLength(200)]
    public string? Aciklama { get; set; }

    [Required]
    [ForeignKey("Kategori")]
    public int KategoriID { get; set; }
    public virtual Kategori? Kategori { get; set; }

    [Required]
    [ForeignKey("Kullanici")]
    public int KullaniciID { get; set; }
    public virtual Kullanici? Kullanici { get; set; }
  }
}