namespace PemesananTiketKonser.Models;

// Data untuk tabel di halaman Kategori: kategori + jumlah terjual + sisa kuota.
// Bukan tabel database, hanya dipakai untuk tampilan.
public class KategoriDenganSisa
{
    public KategoriTiket Kategori { get; set; } = new();
    public int Terjual { get; set; }
    public int Sisa => Kategori.Kuota - Terjual;
}
