using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public class Bildirim
  {
    [Key]
    public int BildirimID { get; set; }

    [Required]
    public string Mesaj { get; set; } = string.Empty;

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

    public bool Okundu { get; set; }

    public bool Yildizli { get; set; }

    [ForeignKey("Kullanici")]
    public int KullaniciID { get; set; }
    public virtual Kullanici? Kullanici { get; set; }
  }
}