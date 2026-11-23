using System.ComponentModel.DataAnnotations;

namespace PemesananTiketKonser.Dtos;

// Isi body JSON untuk POST dan PUT /api/kategori.
// Id tidak ada di sini supaya pemanggil tidak bisa memilih Id sendiri.
public class KategoriWriteRequest
{
    [Required(ErrorMessage = "Nama kategori wajib diisi.")]
    [StringLength(50)]
    public string NamaKategori { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Harga harus lebih dari 0.")]
    public int Harga { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Kuota tidak boleh negatif.")]
    public int Kuota { get; set; }
}
