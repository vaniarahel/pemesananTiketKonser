using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PemesananTiketKonser.Models;

// Transaksi pemesanan tiket.
[Table("pesanan")]
public class Pesanan
{
    // Nilai status yang dipakai di seluruh aplikasi.
    public const string StatusLunas = "LUNAS";
    public const string StatusBelumLunas = "BELUM LUNAS";
    public const int MaksTiketPerPesanan = 4;

    public int Id { get; set; }

    // Kode yang dilihat pengguna, contoh: TK-0001 (dibuat otomatis oleh aplikasi).
    [Required, StringLength(20)]
    [Display(Name = "ID Pesanan")]
    public string IdPesanan { get; set; } = "";

    [Display(Name = "Pembeli")]
    public int PembeliId { get; set; }
    public Pembeli? Pembeli { get; set; }

    [Display(Name = "Kategori Tiket")]
    public int KategoriId { get; set; }
    public KategoriTiket? Kategori { get; set; }

    [Range(1, MaksTiketPerPesanan, ErrorMessage = "Jumlah tiket 1 sampai 4 per pesanan.")]
    [Display(Name = "Jumlah Tiket")]
    public int JumlahTiket { get; set; }

    // harga kategori x jumlah tiket, dihitung di server.
    [Display(Name = "Total Harga (Rp)")]
    public int TotalHarga { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Pembayaran tidak boleh negatif.")]
    [Display(Name = "Pembayaran (Rp)")]
    public int Pembayaran { get; set; }

    // LUNAS kalau Pembayaran >= TotalHarga, selain itu BELUM LUNAS.
    [Required, StringLength(20)]
    public string Status { get; set; } = StatusBelumLunas;

    [Display(Name = "Tanggal Pesan")]
    public DateTime TglPesan { get; set; }
}