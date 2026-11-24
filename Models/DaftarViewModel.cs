using System.ComponentModel.DataAnnotations;

namespace PemesananTiketKonser.Models;

// Data form Daftar Akun pembeli (bukan tabel database).
public class DaftarViewModel
{
    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(100)]
    public string Nama { get; set; } = "";

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(100)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "No HP wajib diisi.")]
    [StringLength(20)]
    [Display(Name = "No HP")]
    public string NoHp { get; set; } = "";

    [Required(ErrorMessage = "No identitas wajib diisi.")]
    [StringLength(20)]
    [Display(Name = "No Identitas (KTP/KTM)")]
    public string NoIdentitas { get; set; } = "";

    [Required(ErrorMessage = "Password wajib diisi.")]
    [StringLength(64, MinimumLength = 6, ErrorMessage = "Password 6 sampai 64 karakter.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Ulangi password.")]
    [Compare(nameof(Password), ErrorMessage = "Password tidak sama.")]
    [DataType(DataType.Password)]
    [Display(Name = "Ulangi Password")]
    public string UlangiPassword { get; set; } = "";
}
