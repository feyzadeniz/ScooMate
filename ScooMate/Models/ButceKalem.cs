using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
  public class ButceKalem
  {
    [Key]
    public int ButceKalemID { get; set; }

    [Required]
    [StringLength(100)]
    public string Aciklama { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Tutar { get; set; }

    [Required]
    public bool IsGelir { get; set; } // true: gelir, false: gider

    [ForeignKey("ButcePlanlama")]
    public int ButcePlanlamaID { get; set; }
    public virtual ButcePlanlama ButcePlanlama { get; set; } = null!;
  }
}