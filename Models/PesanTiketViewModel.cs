using System.ComponentModel.DataAnnotations;

namespace PemesananTiketKonser.Models;

// Data form Pesan Tiket milik pembeli (pembeli tidak memilih nama pembeli dan tidak mengisi pembayaran).
public class PesanTiketViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Pilih kategori tiket.")]
    [Display(Name = "Kategori Tiket")]
    public int KategoriId { get; set; }

    [Range(1, Pesanan.MaksTiketPerPesanan, ErrorMessage = "Jumlah tiket 1 sampai 4 per pesanan.")]
    [Display(Name = "Jumlah Tiket")]
    public int JumlahTiket { get; set; } = 1;
}
