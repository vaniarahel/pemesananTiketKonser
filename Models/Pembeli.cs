using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PemesananTiketKonser.Models;

// Data master pembeli tiket.
[Table("pembeli")]
public class Pembeli
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(100)]
    public string Nama { get; set; } = "";

    // Satu email hanya untuk satu pembeli (unique index di ApplicationDbContext).
    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(100)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "No HP wajib diisi.")]
    [StringLength(20)]
    [Display(Name = "No HP")]
    public string NoHp { get; set; } = "";

    [Required(ErrorMessage = "No identitas wajib diisi.")]
    [StringLength(20)]
    [Display(Name = "No Identitas (KTP/KTM)")]
    public string NoIdentitas { get; set; } = "";
}