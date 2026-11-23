using System.ComponentModel.DataAnnotations;

namespace PemesananTiketKonser.Dtos;

// Isi body JSON untuk POST dan PUT /api/pesanan.
// ID pesanan, total harga, status, dan tanggal TIDAK ada di sini: semuanya dibuat server.
public class PesananWriteRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "PembeliId wajib diisi.")]
    public int PembeliId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "KategoriId wajib diisi.")]
    public int KategoriId { get; set; }

    [Range(1, 4, ErrorMessage = "Jumlah tiket 1 sampai 4 per pesanan.")]
    public int JumlahTiket { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Pembayaran tidak boleh negatif.")]
    public int Pembayaran { get; set; }
}
