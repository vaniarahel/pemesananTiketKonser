using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Dtos;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// REST API Pesanan. Aturan bisnisnya (ID otomatis, kuota, total, status) sama dengan
// halaman Pemesanan karena sama-sama memakai PesananLogic.
// Catatan: seperti di modul praktikum, API ini belum memakai login.
[ApiController]
[Route("api/pesanan")]
public class PesananApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PesananApiController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /api/pesanan
    [HttpGet]
    public async Task<ActionResult<List<PesananResponse>>> GetAll()
    {
        var data = await _db.Pesanan
            .AsNoTracking()
            .Include(p => p.Pembeli)
            .Include(p => p.Kategori)
            .OrderBy(p => p.IdPesanan)
            .ToListAsync();

        return Ok(data.Select(KeResponse).ToList());
    }

    // GET /api/pesanan/TK-0001
    [HttpGet("{idPesanan}")]
    public async Task<ActionResult<PesananResponse>> GetByIdPesanan(string idPesanan)
    {
        var pesanan = await _db.Pesanan
            .AsNoTracking()
            .Include(p => p.Pembeli)
            .Include(p => p.Kategori)
            .FirstOrDefaultAsync(p => p.IdPesanan == idPesanan);

        return pesanan == null
            ? NotFound(new { message = "Pesanan tidak ditemukan." })
            : Ok(KeResponse(pesanan));
    }

    // POST /api/pesanan
    [HttpPost]
    public async Task<ActionResult<PesananResponse>> Create(PesananWriteRequest request)
    {
        var pesanan = new Pesanan
        {
            PembeliId = request.PembeliId,
            KategoriId = request.KategoriId,
            JumlahTiket = request.JumlahTiket,
            Pembayaran = request.Pembayaran
        };

        var error = await ValidasiAsync(pesanan, kecualiPesananId: null);
        if (error != null)
        {
            return error;
        }

        pesanan.TglPesan = DateTime.Now;

        // Kalau ada request lain yang menyimpan bersamaan, ID bisa kembar (error 1062): coba ID berikutnya.
        for (var percobaan = 1; ; percobaan++)
        {
            pesanan.IdPesanan = await _db.BuatIdPesananAsync();
            _db.Pesanan.Add(pesanan);
            try
            {
                await _db.SaveChangesAsync();
                break;
            }
            catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex))
            {
                _db.Entry(pesanan).State = EntityState.Detached;
                if (percobaan >= 3)
                {
                    return Conflict(new { message = "ID pesanan bentrok, silakan coba lagi." });
                }
            }
        }

        var hasil = await _db.Pesanan
            .AsNoTracking()
            .Include(p => p.Pembeli)
            .Include(p => p.Kategori)
            .FirstAsync(p => p.Id == pesanan.Id);

        // 201 Created + header Location: /api/pesanan/{idPesanan}
        return CreatedAtAction(nameof(GetByIdPesanan), new { idPesanan = hasil.IdPesanan }, KeResponse(hasil));
    }

    // PUT /api/pesanan/TK-0001
    [HttpPut("{idPesanan}")]
    public async Task<IActionResult> Update(string idPesanan, PesananWriteRequest request)
    {
        var pesanan = await _db.Pesanan.FirstOrDefaultAsync(p => p.IdPesanan == idPesanan);
        if (pesanan == null)
        {
            return NotFound(new { message = "Pesanan tidak ditemukan." });
        }

        var baru = new Pesanan
        {
            PembeliId = request.PembeliId,
            KategoriId = request.KategoriId,
            JumlahTiket = request.JumlahTiket,
            Pembayaran = request.Pembayaran
        };

        // Kuota dihitung tanpa tiket milik pesanan ini sendiri.
        var error = await ValidasiAsync(baru, kecualiPesananId: pesanan.Id);
        if (error != null)
        {
            return error;
        }

        pesanan.PembeliId = baru.PembeliId;
        pesanan.KategoriId = baru.KategoriId;
        pesanan.JumlahTiket = baru.JumlahTiket;
        pesanan.Pembayaran = baru.Pembayaran;
        pesanan.TotalHarga = baru.TotalHarga;
        pesanan.Status = baru.Status;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/pesanan/TK-0001
    [HttpDelete("{idPesanan}")]
    public async Task<IActionResult> Delete(string idPesanan)
    {
        var pesanan = await _db.Pesanan.FirstOrDefaultAsync(p => p.IdPesanan == idPesanan);
        if (pesanan == null)
        {
            return NotFound(new { message = "Pesanan tidak ditemukan." });
        }

        _db.Pesanan.Remove(pesanan);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Pembeli/kategori tidak ada -> 404. Kuota tidak cukup -> 400. Valid -> null.
    private async Task<ActionResult?> ValidasiAsync(Pesanan pesanan, int? kecualiPesananId)
    {
        var errors = await _db.ValidasiDanHitungAsync(pesanan, kecualiPesananId);
        if (errors.Count == 0)
        {
            return null;
        }

        var tidakAda = errors.FirstOrDefault(e =>
            e.Field == nameof(Pesanan.PembeliId) || e.Field == nameof(Pesanan.KategoriId));

        if (tidakAda != default)
        {
            var pesan = tidakAda.Field == nameof(Pesanan.PembeliId)
                ? "Pembeli tidak ditemukan."
                : "Kategori tidak ditemukan.";
            return NotFound(new { message = pesan });
        }

        return BadRequest(new { message = errors[0].Pesan });
    }

    private static PesananResponse KeResponse(Pesanan p) => new()
    {
        IdPesanan = p.IdPesanan,
        TglPesan = p.TglPesan,
        PembeliId = p.PembeliId,
        NamaPembeli = p.Pembeli?.Nama ?? "",
        Email = p.Pembeli?.Email ?? "",
        KategoriId = p.KategoriId,
        NamaKategori = p.Kategori?.NamaKategori ?? "",
        JumlahTiket = p.JumlahTiket,
        TotalHarga = p.TotalHarga,
        Pembayaran = p.Pembayaran,
        Kekurangan = Math.Max(0, p.TotalHarga - p.Pembayaran),
        Kembalian = PesananLogic.HitungKembalian(p.TotalHarga, p.Pembayaran),
        Status = p.Status
    };
}
