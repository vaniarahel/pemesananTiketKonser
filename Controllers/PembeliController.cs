using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// CRUD Data Pembeli + Cari. Semua halaman wajib login.
[WajibLogin(DaftarPeran.Admin)]
public class PembeliController : Controller
{
    private readonly ApplicationDbContext _db;

    public PembeliController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /Pembeli?cari=andi : daftar pembeli, bisa dicari berdasarkan nama, email, no HP, atau no identitas.
    public async Task<IActionResult> Index(string? cari)
    {
        var query = _db.Pembeli.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(cari))
        {
            cari = cari.Trim();
            query = query.Where(p =>
                p.Nama.Contains(cari) ||
                p.Email.Contains(cari) ||
                p.NoHp.Contains(cari) ||
                p.NoIdentitas.Contains(cari));
        }

        ViewBag.Cari = cari;
        return View(await query.OrderBy(p => p.Nama).ToListAsync());
    }

    // GET /Pembeli/Create : form tambah.
    public IActionResult Create()
    {
        return View(new Pembeli());
    }

    // POST /Pembeli/Create : simpan pembeli baru.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nama,Email,NoHp,NoIdentitas")] Pembeli model)
    {
        if (await _db.Pembeli.AnyAsync(p => p.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email sudah terdaftar.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.Pembeli.Add(model);
        if (!await SimpanAsync(model))
        {
            return View(model);
        }

        TempData["Sukses"] = $"Pembeli \"{model.Nama}\" berhasil ditambahkan.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Pembeli/Edit/5 : form ubah.
    public async Task<IActionResult> Edit(int id)
    {
        var pembeli = await _db.Pembeli.FindAsync(id);
        if (pembeli == null)
        {
            return NotFound();
        }

        return View(pembeli);
    }

    // POST /Pembeli/Edit/5 : simpan perubahan.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nama,Email,NoHp,NoIdentitas")] Pembeli model)
    {
        if (id != model.Id || !await _db.Pembeli.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }

        if (await _db.Pembeli.AnyAsync(p => p.Email == model.Email && p.Id != id))
        {
            ModelState.AddModelError(nameof(model.Email), "Email sudah terdaftar.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.Pembeli.Update(model);
        if (!await SimpanAsync(model))
        {
            return View(model);
        }

        TempData["Sukses"] = $"Pembeli \"{model.Nama}\" berhasil diubah.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Pembeli/Delete/5 : halaman konfirmasi hapus.
    public async Task<IActionResult> Delete(int id)
    {
        var pembeli = await _db.Pembeli.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (pembeli == null)
        {
            return NotFound();
        }

        ViewBag.PunyaPesanan = await _db.Pesanan.AnyAsync(p => p.PembeliId == id);
        return View(pembeli);
    }

    // POST /Pembeli/Delete/5 : hapus pembeli.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pembeli = await _db.Pembeli.FindAsync(id);
        if (pembeli == null)
        {
            return NotFound();
        }

        // Pembeli yang sudah punya pesanan tidak boleh dihapus.
        if (await _db.Pesanan.AnyAsync(p => p.PembeliId == id))
        {
            TempData["Gagal"] = $"Pembeli \"{pembeli.Nama}\" tidak bisa dihapus karena sudah punya pesanan.";
            return RedirectToAction(nameof(Index));
        }

        if (await _db.Admin.AnyAsync(a => a.PembeliId == id))
        {
            TempData["Gagal"] = $"Pembeli \"{pembeli.Nama}\" tidak bisa dihapus karena punya akun login.";
            return RedirectToAction(nameof(Index));
        }

        _db.Pembeli.Remove(pembeli);
        try
        {
            await _db.SaveChangesAsync();
            TempData["Sukses"] = $"Pembeli \"{pembeli.Nama}\" berhasil dihapus.";
        }
        catch (DbUpdateException ex) when (DbHelper.IsForeignKeyViolation(ex))
        {
            TempData["Gagal"] = $"Pembeli \"{pembeli.Nama}\" tidak bisa dihapus karena sudah punya pesanan.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Simpan ke database. Kalau email kembar (misal dua admin menyimpan bersamaan), tampilkan pesan error.
    private async Task<bool> SimpanAsync(Pembeli model)
    {
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex))
        {
            ModelState.AddModelError(nameof(model.Email), "Email sudah terdaftar.");
            return false;
        }
    }
}
