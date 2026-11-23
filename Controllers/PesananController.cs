using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// Pemesanan Tiket: tampil, tambah, edit, hapus. Semua halaman wajib login.
[WajibLogin]
public class PesananController : Controller
{
    private readonly ApplicationDbContext _db;

    public PesananController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /Pesanan : daftar pesanan, terbaru di atas.
    public async Task<IActionResult> Index()
    {
        var data = await _db.Pesanan
            .AsNoTracking()
            .Include(p => p.Pembeli)
            .Include(p => p.Kategori)
            .OrderByDescending(p => p.TglPesan)
            .ToListAsync();

        return View(data);
    }

    // GET /Pesanan/Create : form pesan tiket.
    public async Task<IActionResult> Create()
    {
        ViewBag.IdPesananBerikutnya = await _db.BuatIdPesananAsync();
        await IsiPilihanAsync();
        return View(new Pesanan { JumlahTiket = 1 });
    }

    // POST /Pesanan/Create : simpan pesanan baru.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PembeliId,KategoriId,JumlahTiket,Pembayaran")] Pesanan model)
    {
        // ID pesanan, total, dan status dibuat server, bukan dari form.
        ModelState.Remove(nameof(Pesanan.IdPesanan));
        ModelState.Remove(nameof(Pesanan.Status));

        foreach (var (field, pesan) in await _db.ValidasiDanHitungAsync(model))
        {
            ModelState.AddModelError(field, pesan);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.IdPesananBerikutnya = await _db.BuatIdPesananAsync();
            await IsiPilihanAsync();
            return View(model);
        }

        model.TglPesan = DateTime.Now;

        // Kalau dua admin menyimpan bersamaan, ID bisa kembar (error 1062): coba ID berikutnya.
        for (var percobaan = 1; ; percobaan++)
        {
            model.IdPesanan = await _db.BuatIdPesananAsync();
            _db.Pesanan.Add(model);
            try
            {
                await _db.SaveChangesAsync();
                break;
            }
            catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex) && percobaan < 3)
            {
                _db.Entry(model).State = EntityState.Detached;
            }
        }

        TempData["Sukses"] = $"Pesanan {model.IdPesanan} berhasil disimpan ({PesananLogic.KeteranganBayar(model.TotalHarga, model.Pembayaran)}).";
        return RedirectToAction(nameof(Index));
    }

    // GET /Pesanan/Edit/5 : form ubah pesanan.
    public async Task<IActionResult> Edit(int id)
    {
        var pesanan = await _db.Pesanan.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (pesanan == null)
        {
            return NotFound();
        }

        await IsiPilihanAsync(pesanan.Id);
        return View(pesanan);
    }

    // POST /Pesanan/Edit/5 : simpan perubahan. ID pesanan dan tanggal tidak berubah.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,PembeliId,KategoriId,JumlahTiket,Pembayaran")] Pesanan model)
    {
        var lama = await _db.Pesanan.FirstOrDefaultAsync(p => p.Id == id);
        if (lama == null || id != model.Id)
        {
            return NotFound();
        }

        ModelState.Remove(nameof(Pesanan.IdPesanan));
        ModelState.Remove(nameof(Pesanan.Status));

        // Kuota dihitung tanpa tiket milik pesanan ini sendiri.
        foreach (var (field, pesan) in await _db.ValidasiDanHitungAsync(model, kecualiPesananId: id))
        {
            ModelState.AddModelError(field, pesan);
        }

        if (!ModelState.IsValid)
        {
            model.IdPesanan = lama.IdPesanan;
            model.TglPesan = lama.TglPesan;
            await IsiPilihanAsync(id);
            return View(model);
        }

        lama.PembeliId = model.PembeliId;
        lama.KategoriId = model.KategoriId;
        lama.JumlahTiket = model.JumlahTiket;
        lama.Pembayaran = model.Pembayaran;
        lama.TotalHarga = model.TotalHarga;
        lama.Status = model.Status;
        await _db.SaveChangesAsync();

        TempData["Sukses"] = $"Pesanan {lama.IdPesanan} berhasil diubah ({PesananLogic.KeteranganBayar(lama.TotalHarga, lama.Pembayaran)}).";
        return RedirectToAction(nameof(Index));
    }

    // GET /Pesanan/Delete/5 : halaman konfirmasi hapus.
    public async Task<IActionResult> Delete(int id)
    {
        var pesanan = await _db.Pesanan
            .AsNoTracking()
            .Include(p => p.Pembeli)
            .Include(p => p.Kategori)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pesanan == null)
        {
            return NotFound();
        }

        return View(pesanan);
    }

    // POST /Pesanan/Delete/5 : hapus pesanan (kuota kategorinya otomatis kembali).
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pesanan = await _db.Pesanan.FindAsync(id);
        if (pesanan == null)
        {
            return NotFound();
        }

        _db.Pesanan.Remove(pesanan);
        await _db.SaveChangesAsync();

        TempData["Sukses"] = $"Pesanan {pesanan.IdPesanan} berhasil dihapus.";
        return RedirectToAction(nameof(Index));
    }

    // Isi pilihan dropdown pembeli dan kategori (beserta harga dan sisa kuota).
    private async Task IsiPilihanAsync(int? kecualiPesananId = null)
    {
        var pembeli = await _db.Pembeli.AsNoTracking().OrderBy(p => p.Nama).ToListAsync();
        ViewBag.PilihanPembeli = pembeli
            .Select(p => new SelectListItem($"{p.Nama} ({p.Email})", p.Id.ToString()))
            .ToList();

        var kategori = await _db.KategoriTiket.AsNoTracking().OrderBy(k => k.Id).ToListAsync();
        var pilihan = new List<KategoriPilihan>();
        foreach (var k in kategori)
        {
            pilihan.Add(new KategoriPilihan
            {
                Id = k.Id,
                NamaKategori = k.NamaKategori,
                Harga = k.Harga,
                Sisa = await _db.SisaKuotaAsync(k.Id, kecualiPesananId)
            });
        }
        ViewBag.PilihanKategori = pilihan;
    }
}
