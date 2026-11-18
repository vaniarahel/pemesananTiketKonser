using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PemesananTiketKonser.Models;

// Jenis tiket beserta harga dan kuotanya.
[Table("kategori_tiket")]
public class KategoriTiket
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nama kategori wajib diisi.")]
    [StringLength(50)]
    [Display(Name = "Nama Kategori")]
    public string NamaKategori { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Harga harus lebih dari 0.")]
    [Display(Name = "Harga (Rp)")]
    public int Harga { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Kuota tidak boleh negatif.")]
    public int Kuota { get; set; }
}