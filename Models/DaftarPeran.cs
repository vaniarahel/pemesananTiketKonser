namespace PemesananTiketKonser.Models;

// Daftar peran (role) akun yang dipakai di seluruh aplikasi.
public static class DaftarPeran
{
    // Petugas/pengelola: akses semua menu (pembeli, kategori, pemesanan, laporan).
    public const string Admin = "Admin";

    // Pembeli: hanya bisa memesan tiket atas nama sendiri dan melihat pesanannya sendiri.
    public const string Pembeli = "Pembeli";
}
