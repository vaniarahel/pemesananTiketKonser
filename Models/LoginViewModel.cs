using System.ComponentModel.DataAnnotations;

namespace PemesananTiketKonser.Models;

// Data yang dikirim dari form Login (bukan tabel database).
public class LoginViewModel
{
    [Required(ErrorMessage = "Username/email wajib diisi.")]
    [Display(Name = "Username / Email")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Password wajib diisi.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}
