namespace PemesananTiketKonser.Models;

// Satu pilihan kategori di dropdown halaman Pemesanan (harga + sisa kuota).
public class KategoriPilihan
{
    public int Id { get; set; }
    public string NamaKategori { get; set; } = "";
    public int Harga { get; set; }
    public int Sisa { get; set; }
}