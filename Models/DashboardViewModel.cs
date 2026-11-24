namespace PemesananTiketKonser.Models;

// Data untuk halaman Menu Utama / Dashboard (bukan tabel database).
public class DashboardViewModel
{
    public int TotalPembeli { get; set; }
    public int TotalPesanan { get; set; }
    public int TotalTiket { get; set; }
    public int PesananBelumLunas { get; set; }

    // Uang yang benar-benar diterima (tanpa kembalian).
    public int TotalPendapatan { get; set; }

    // Sisa tagihan pesanan yang BELUM LUNAS.
    public int BelumDibayar { get; set; }

    public List<KategoriDenganSisa> Kuota { get; set; } = new();

    // Pesanan BELUM LUNAS, yang paling lama di atas (paling perlu ditagih).
    public List<Pesanan> PerluDitagih { get; set; } = new();
}