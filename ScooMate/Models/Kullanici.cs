using System.ComponentModel.DataAnnotations;

namespace ScooMate.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public required string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public required string Soyad { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Telefon numarası 10 haneli olmalıdır")]
        public required string Telefon { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [DataType(DataType.Password)]
        public required string Sifre { get; set; }
    }
}

