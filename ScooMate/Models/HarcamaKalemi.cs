using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public class HarcamaKalemi
  {
    [Key]
    public int HarcamaKalemiId { get; set; }

    [Required]
    [StringLength(100)]
    public string Ad { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Kategori { get; set; } = string.Empty;

    [Required]
    public int KullaniciID { get; set; }
    public virtual Kullanici? Kullanici { get; set; }

    
    public virtual ICollection<Harcama> Harcamalar { get; set; } = new List<Harcama>();
  }
}