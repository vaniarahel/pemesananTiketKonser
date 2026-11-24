namespace PemesananTiketKonser.Dtos;

// Bentuk JSON pesanan yang dikirim API.
public class PesananResponse
{
    public string IdPesanan { get; set; } = "";
    public DateTime TglPesan { get; set; }
    public int PembeliId { get; set; }
    public string NamaPembeli { get; set; } = "";
    public string Email { get; set; } = "";
    public int KategoriId { get; set; }
    public string NamaKategori { get; set; } = "";
    public int JumlahTiket { get; set; }
    public int TotalHarga { get; set; }
    public int Pembayaran { get; set; }
    public int Kekurangan { get; set; }
    public int Kembalian { get; set; }
    public string Status { get; set; } = "";
}
