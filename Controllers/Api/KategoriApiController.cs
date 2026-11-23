using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Dtos;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// REST API Kategori Tiket. Mengembalikan JSON, bukan halaman HTML.
// Catatan: seperti di modul praktikum, API ini belum memakai login.
[ApiController]
[Route("api/kategori")]
public class KategoriApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public KategoriApiController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /api/kategori
    [HttpGet]
    public async Task<ActionResult<List<KategoriResponse>>> GetAll()
    {
        var kategori = await _db.KategoriTiket.AsNoTracking().OrderBy(k => k.Id).ToListAsync();
        var terjual = await _db.TiketTerjualPerKategoriAsync();

        return Ok(kategori.Select(k => KeResponse(k, terjual.GetValueOrDefault(k.Id))).ToList());
    }

    // GET /api/kategori/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<KategoriResponse>> GetById(int id)
    {
        var kategori = await _db.KategoriTiket.AsNoTracking().FirstOrDefaultAsync(k => k.Id == id);
        if (kategori == null)
        {
            return NotFound(new { message = "Kategori tidak ditemukan." });
        }

        var terjual = (await _db.TiketTerjualPerKategoriAsync()).GetValueOrDefault(id);
        return Ok(KeResponse(kategori, terjual));
    }

    // POST /api/kategori
    [HttpPost]
    public async Task<ActionResult<KategoriResponse>> Create(KategoriWriteRequest request)
    {
        if (await _db.KategoriTiket.AnyAsync(k => k.NamaKategori == request.NamaKategori))
        {
            return Conflict(new { message = "Nama kategori sudah ada." });
        }

        var kategori = new KategoriTiket
        {
            NamaKategori = request.NamaKategori,
            Harga = request.Harga,
            Kuota = request.Kuota
        };

        _db.KategoriTiket.Add(kategori);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex))
        {
            return Conflict(new { message = "Nama kategori sudah ada." });
        }

        // 201 Created + header Location: /api/kategori/{id}
        return CreatedAtAction(nameof(GetById), new { id = kategori.Id }, KeResponse(kategori, 0));
    }

    // PUT /api/kategori/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, KategoriWriteRequest request)
    {
        var kategori = await _db.KategoriTiket.FirstOrDefaultAsync(k => k.Id == id);
        if (kategori == null)
        {
            return NotFound(new { message = "Kategori tidak ditemukan." });
        }

        if (await _db.KategoriTiket.AnyAsync(k => k.NamaKategori == request.NamaKategori && k.Id != id))
        {
            return Conflict(new { message = "Nama kategori sudah ada." });
        }

        var terjual = (await _db.TiketTerjualPerKategoriAsync()).GetValueOrDefault(id);
        if (request.Kuota < terjual)
        {
            return BadRequest(new { message = $"Kuota minimal {terjual}, karena sudah terjual {terjual} tiket." });
        }

        kategori.NamaKategori = request.NamaKategori;
        kategori.Harga = request.Harga;
        kategori.Kuota = request.Kuota;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex))
        {
            return Conflict(new { message = "Nama kategori sudah ada." });
        }

        return NoContent();
    }

    // DELETE /api/kategori/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var kategori = await _db.KategoriTiket.FirstOrDefaultAsync(k => k.Id == id);
        if (kategori == null)
        {
            return NotFound(new { message = "Kategori tidak ditemukan." });
        }

        if (await _db.Pesanan.AnyAsync(p => p.KategoriId == id))
        {
            return Conflict(new { message = "Kategori tidak bisa dihapus karena sudah dipakai pesanan." });
        }

        _db.KategoriTiket.Remove(kategori);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (DbHelper.IsForeignKeyViolation(ex))
        {
            return Conflict(new { message = "Kategori tidak bisa dihapus karena sudah dipakai pesanan." });
        }

        return NoContent();
    }

    private static KategoriResponse KeResponse(KategoriTiket k, int terjual) => new()
    {
        Id = k.Id,
        NamaKategori = k.NamaKategori,
        Harga = k.Harga,
        Kuota = k.Kuota,
        Terjual = terjual,
        SisaKuota = k.Kuota - terjual
    };
}
