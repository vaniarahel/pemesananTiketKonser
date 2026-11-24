namespace PemesananTiketKonser.Models;

// Data untuk halaman Laporan Penjualan (bukan tabel database).
public class LaporanViewModel
{
    // Filter
    public DateTime? Dari { get; set; }
    public DateTime? Sampai { get; set; }
    public string? Status { get; set; }

    // Isi laporan
    public List<Pesanan> Transaksi { get; set; } = new();
    public List<LaporanPerKategori> PerKategori { get; set; } = new();

    // Total keseluruhan
    public int TotalPesanan => Transaksi.Count;
    public int TotalTiket => Transaksi.Sum(p => p.JumlahTiket);
    public int NilaiPenjualan => Transaksi.Sum(p => p.TotalHarga);
    public int UangMasuk => Transaksi.Sum(p => Math.Min(p.Pembayaran, p.TotalHarga));
    public int Piutang => Transaksi.Sum(p => Math.Max(0, p.TotalHarga - p.Pembayaran));
}

// Satu baris ringkasan per kategori tiket.
public class LaporanPerKategori
{
    public string NamaKategori { get; set; } = "";
    public int JumlahPesanan { get; set; }
    public int TiketTerjual { get; set; }

    // Nilai penjualan = jumlah TotalHarga (harga x jumlah tiket).
    public int NilaiPenjualan { get; set; }

    // Uang yang benar-benar diterima (kelebihan bayar / kembalian tidak dihitung).
    public int UangMasuk { get; set; }

    // Sisa tagihan pesanan yang BELUM LUNAS.
    public int Piutang { get; set; }
}
