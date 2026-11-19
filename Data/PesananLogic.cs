using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Data;

// Aturan bisnis pemesanan tiket. Dipakai oleh PesananController (MVC)
// dan nanti juga oleh PesananApiController (REST API), supaya aturannya sama persis.
public static class PesananLogic
{
    private const string Prefix = "TK-";

    // Membuat ID pesanan berikutnya: TK-0001, TK-0002, dst.
    public static async Task<string> BuatIdPesananAsync(this ApplicationDbContext db)
    {
        var semuaId = await db.Pesanan.Select(p => p.IdPesanan).ToListAsync();

        var terbesar = semuaId
            .Where(id => id.StartsWith(Prefix))
            .Select(id => int.TryParse(id[Prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return Prefix + (terbesar + 1).ToString("D4");
    }

    // Sisa kuota sebuah kategori.
    // kecualiPesananId dipakai saat EDIT, supaya tiket milik pesanan itu sendiri tidak ikut dihitung.
    public static async Task<int> SisaKuotaAsync(this ApplicationDbContext db, int kategoriId, int? kecualiPesananId = null)
    {
        var kategori = await db.KategoriTiket.AsNoTracking().FirstOrDefaultAsync(k => k.Id == kategoriId);
        if (kategori == null)
        {
            return 0;
        }

        var terjual = await db.Pesanan
            .Where(p => p.KategoriId == kategoriId && p.Id != (kecualiPesananId ?? 0))
            .SumAsync(p => p.JumlahTiket);

        return kategori.Kuota - terjual;
    }

    // LUNAS kalau pembayaran sudah menutup total harga.
    public static string HitungStatus(int totalHarga, int pembayaran) =>
        pembayaran >= totalHarga ? Pesanan.StatusLunas : Pesanan.StatusBelumLunas;

    // Kelebihan bayar yang harus dikembalikan ke pembeli (0 kalau tidak lebih).
    public static int HitungKembalian(int totalHarga, int pembayaran) =>
        Math.Max(0, pembayaran - totalHarga);

    // Pesan status untuk ditampilkan: LUNAS, LUNAS (kembalian ...), atau BELUM LUNAS (kurang ...).
    public static string KeteranganBayar(int totalHarga, int pembayaran)
    {
        if (pembayaran > totalHarga)
        {
            return $"LUNAS, kembalian {Helpers.Format.Rupiah(pembayaran - totalHarga)}";
        }

        return pembayaran == totalHarga
            ? "LUNAS"
            : $"BELUM LUNAS, kurang {Helpers.Format.Rupiah(totalHarga - pembayaran)}";
    }

    // Cek pembeli, kategori, dan kuota, lalu isi TotalHarga dan Status.
    // Mengembalikan daftar error (nama field, pesan). Kosong = valid.
    public static async Task<List<(string Field, string Pesan)>> ValidasiDanHitungAsync(
        this ApplicationDbContext db, Pesanan pesanan, int? kecualiPesananId = null)
    {
        var errors = new List<(string, string)>();

        if (!await db.Pembeli.AnyAsync(p => p.Id == pesanan.PembeliId))
        {
            errors.Add((nameof(Pesanan.PembeliId), "Pilih pembeli."));
        }

        var kategori = await db.KategoriTiket.AsNoTracking().FirstOrDefaultAsync(k => k.Id == pesanan.KategoriId);
        if (kategori == null)
        {
            errors.Add((nameof(Pesanan.KategoriId), "Pilih kategori tiket."));
            return errors;
        }

        var sisa = await db.SisaKuotaAsync(pesanan.KategoriId, kecualiPesananId);
        if (pesanan.JumlahTiket > sisa)
        {
            errors.Add((nameof(Pesanan.JumlahTiket),
                sisa <= 0
                    ? $"Kuota kategori {kategori.NamaKategori} sudah habis."
                    : $"Sisa kuota {kategori.NamaKategori} tinggal {sisa} tiket."));
        }

        // Total dan status selalu dihitung server, tidak diambil dari input pengguna.
        pesanan.TotalHarga = kategori.Harga * pesanan.JumlahTiket;
        pesanan.Status = HitungStatus(pesanan.TotalHarga, pesanan.Pembayaran);

        return errors;
    }
}