using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
    public class ButcePlanlama
    {
        [Key]
        public int ButcePlanlamaID { get; set; }

        [Required]
        public DateTime Tarih { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gelirler { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TemelGiderler { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Kullanılabilir Limit")]
        public decimal KullanilabilirLimit { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciID { get; set; }
        public virtual Kullanici Kullanici { get; set; } = null!;

        public virtual ICollection<ButceKalem> Kalemler { get; set; } = new List<ButceKalem>();

        public void LimitHesapla()
        {
            Gelirler = Kalemler.Where(k => k.IsGelir).Sum(k => k.Tutar);
            TemelGiderler = Kalemler.Where(k => !k.IsGelir).Sum(k => k.Tutar);

            KullanilabilirLimit = Gelirler - TemelGiderler;
        }
    }
}
