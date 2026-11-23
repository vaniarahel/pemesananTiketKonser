namespace PemesananTiketKonser.Dtos;

// Bentuk JSON kategori yang dikirim API (sudah termasuk tiket terjual dan sisa kuota).
public class KategoriResponse
{
    public int Id { get; set; }
    public string NamaKategori { get; set; } = "";
    public int Harga { get; set; }
    public int Kuota { get; set; }
    public int Terjual { get; set; }
    public int SisaKuota { get; set; }
}
