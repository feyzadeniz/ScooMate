using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScooMate.Models
{
    public class Yatirim
    {
        [Key]
        public int YatirimID { get; set; }

        [Required]
        public int KullaniciID { get; set; }
        public virtual Kullanici? Kullanici { get; set; }

        [Required]
        public YatirimTuru Tur { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Miktar { get; set; }

        [Required]
        public DateTime Tarih { get; set; }

        [StringLength(250)]
        public string? Aciklama { get; set; }
    }

    public enum YatirimTuru
    {
        Hisse,
        Altin,
        Temettu,
        DovizEuro,
        DovizDolar,
        KriptoParalar,
        Tahvil,
        Bono,
        GayriMenkul,
        Fon,
        VadeliMevduat
    }
}
