using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public class Kategori
  {
    public Kategori()
    {
      KategoriAdi = string.Empty;
      Harcamalar = new List<Harcama>();
    }

    [Key]
    public int KategoriID { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur")]
    [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir")]
    public string KategoriAdi { get; set; }

    [StringLength(200)]
    public string? Aciklama { get; set; }

    public string? OzelSimgeDosyasi { get; set; }

    [ForeignKey("Kullanici")]
    public int? KullaniciID { get; set; }  // Null olabilir, null ise default kategoridir
    public virtual Kullanici? Kullanici { get; set; }

    public virtual ICollection<Harcama> Harcamalar { get; set; } = new List<Harcama>();

    public string? Simge { get; set; }
  }
}