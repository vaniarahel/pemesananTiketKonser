using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PemesananTiketKonser.Models;

// Akun untuk Login. Dipakai oleh admin maupun pembeli.
// (Nama class/tabel tetap "admin" supaya tidak perlu mengubah kode dan tabel yang sudah ada.)
[Table("admin")]
public class Admin
{
    public int Id { get; set; }

    // Admin login pakai username; pembeli login pakai email (disimpan juga di sini).
    [Required, StringLength(100)]
    public string Username { get; set; } = "";

    // Password disimpan apa adanya (teks biasa), maksimal 64 karakter.
    [Required, StringLength(64)]
    public string Password { get; set; } = "";

    // Peran akun: "Admin" atau "Pembeli" (lihat DaftarPeran).
    [Required, StringLength(20)]
    public string Peran { get; set; } = DaftarPeran.Pembeli;

    // Hanya diisi untuk akun pembeli: menunjuk ke data pembeli miliknya.
    public int? PembeliId { get; set; }
    public Pembeli? Pembeli { get; set; }
}
