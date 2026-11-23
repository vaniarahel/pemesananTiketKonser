using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// CRUD Kategori Tiket. Semua halaman wajib login.
[WajibLogin]
public class KategoriController : Controller
{
    private readonly ApplicationDbContext _db;

    public KategoriController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /Kategori : daftar kategori + tiket terjual + sisa kuota.
    public async Task<IActionResult> Index()
    {
        var kategori = await _db.KategoriTiket.AsNoTracking().OrderBy(k => k.Id).ToListAsync();
        var terjual = await _db.TiketTerjualPerKategoriAsync();

        var data = kategori.Select(k => new KategoriDenganSisa
        {
            Kategori = k,
            Terjual = terjual.GetValueOrDefault(k.Id)
        }).ToList();

        return View(data);
    }

    // GET /Kategori/Create : form tambah.
    public IActionResult Create()
    {
        return View(new KategoriTiket());
    }

    // POST /Kategori/Create : simpan kategori baru.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NamaKategori,Harga,Kuota")] KategoriTiket model)
    {
        if (await _db.KategoriTiket.AnyAsync(k => k.NamaKategori == model.NamaKategori))
        {
            ModelState.AddModelError(nameof(model.NamaKategori), "Nama kategori sudah ada.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.KategoriTiket.Add(model);
        if (!await SimpanAsync(model))
        {
            return View(model);
        }

        TempData["Sukses"] = $"Kategori \"{model.NamaKategori}\" berhasil ditambahkan.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Kategori/Edit/5 : form ubah.
    public async Task<IActionResult> Edit(int id)
    {
        var kategori = await _db.KategoriTiket.FindAsync(id);
        if (kategori == null)
        {
            return NotFound();
        }

        return View(kategori);
    }

    // POST /Kategori/Edit/5 : simpan perubahan.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,NamaKategori,Harga,Kuota")] KategoriTiket model)
    {
        if (id != model.Id || !await _db.KategoriTiket.AnyAsync(k => k.Id == id))
        {
            return NotFound();
        }

        if (await _db.KategoriTiket.AnyAsync(k => k.NamaKategori == model.NamaKategori && k.Id != id))
        {
            ModelState.AddModelError(nameof(model.NamaKategori), "Nama kategori sudah ada.");
        }

        // Kuota tidak boleh lebih kecil dari tiket yang sudah terjual.
        var terjual = (await _db.TiketTerjualPerKategoriAsync()).GetValueOrDefault(id);
        if (model.Kuota < terjual)
        {
            ModelState.AddModelError(nameof(model.Kuota), $"Kuota minimal {terjual}, karena sudah terjual {terjual} tiket.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.KategoriTiket.Update(model);
        if (!await SimpanAsync(model))
        {
            return View(model);
        }

        TempData["Sukses"] = $"Kategori \"{model.NamaKategori}\" berhasil diubah.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Kategori/Delete/5 : halaman konfirmasi hapus.
    public async Task<IActionResult> Delete(int id)
    {
        var kategori = await _db.KategoriTiket.AsNoTracking().FirstOrDefaultAsync(k => k.Id == id);
        if (kategori == null)
        {
            return NotFound();
        }

        ViewBag.DipakaiPesanan = await _db.Pesanan.AnyAsync(p => p.KategoriId == id);
        return View(kategori);
    }

    // POST /Kategori/Delete/5 : hapus kategori.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var kategori = await _db.KategoriTiket.FindAsync(id);
        if (kategori == null)
        {
            return NotFound();
        }

        // Kategori yang sudah dipakai pesanan tidak boleh dihapus.
        if (await _db.Pesanan.AnyAsync(p => p.KategoriId == id))
        {
            TempData["Gagal"] = $"Kategori \"{kategori.NamaKategori}\" tidak bisa dihapus karena sudah dipakai pesanan.";
            return RedirectToAction(nameof(Index));
        }

        _db.KategoriTiket.Remove(kategori);
        try
        {
            await _db.SaveChangesAsync();
            TempData["Sukses"] = $"Kategori \"{kategori.NamaKategori}\" berhasil dihapus.";
        }
        catch (DbUpdateException ex) when (DbHelper.IsForeignKeyViolation(ex))
        {
            TempData["Gagal"] = $"Kategori \"{kategori.NamaKategori}\" tidak bisa dihapus karena sudah dipakai pesanan.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Simpan ke database. Kalau nama kembar (misal dua admin menyimpan bersamaan), tampilkan pesan error.
    private async Task<bool> SimpanAsync(KategoriTiket model)
    {
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex))
        {
            ModelState.AddModelError(nameof(model.NamaKategori), "Nama kategori sudah ada.");
            return false;
        }
    }
}
