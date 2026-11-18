using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PemesananTiketKonser.Models;

// Akun admin untuk Login.
[Table("admin")]
public class Admin
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; } = "";

    // Password disimpan sebagai hash SHA-256 (64 karakter hex), bukan teks biasa.
    [Required, StringLength(64)]
    public string Password { get; set; } = "";
}