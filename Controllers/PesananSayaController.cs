using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Helpers;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// Halaman khusus pembeli: pesan tiket atas nama sendiri dan lihat pesanan sendiri.
[WajibLogin(DaftarPeran.Pembeli)]
public class PesananSayaController : Controller
{
    private readonly ApplicationDbContext _db;

    public PesananSayaController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /PesananSaya : daftar pesanan milik pembeli yang login.
    public async Task<IActionResult> Index()
    {
        var pembeliId = HttpContext.PembeliId();
        var data = await _db.Pesanan
            .AsNoTracking()
            .Include(p => p.Kategori)
            .Where(p => p.PembeliId == pembeliId)
            .OrderByDescending(p => p.TglPesan)
            .ToListAsync();

        return View(data);
    }

    // GET /PesananSaya/Create : form pesan tiket.
    public async Task<IActionResult> Create()
    {
        await IsiPilihanKategoriAsync();
        return View(new PesanTiketViewModel());
    }

    // POST /PesananSaya/Create : simpan pesanan atas nama pembeli yang login.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PesanTiketViewModel model)
    {
        var pesanan = new Pesanan
        {
            PembeliId = HttpContext.PembeliId() ?? 0,
            KategoriId = model.KategoriId,
            JumlahTiket = model.JumlahTiket,
            Pembayaran = 0 // dibayar di loket / dicatat admin
        };

        if (ModelState.IsValid)
        {
            foreach (var (field, pesan) in await _db.ValidasiDanHitungAsync(pesanan))
            {
                // Error pembeli tidak ditampilkan di field mana pun (pembeli tidak memilihnya sendiri).
                ModelState.AddModelError(field == nameof(Pesanan.PembeliId) ? string.Empty : field, pesan);
            }
        }

        if (!ModelState.IsValid)
        {
            await IsiPilihanKategoriAsync();
            return View(model);
        }

        pesanan.TglPesan = DateTime.Now;

        // Kalau ada yang menyimpan bersamaan, ID bisa kembar (error 1062): coba ID berikutnya.
        for (var percobaan = 1; ; percobaan++)
        {
            pesanan.IdPesanan = await _db.BuatIdPesananAsync();
            _db.Pesanan.Add(pesanan);
            try
            {
                await _db.SaveChangesAsync();
                break;
            }
            catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex) && percobaan < 3)
            {
                _db.Entry(pesanan).State = EntityState.Detached;
            }
        }

        TempData["Sukses"] = $"Pesanan {pesanan.IdPesanan} berhasil dibuat. Total {Format.Rupiah(pesanan.TotalHarga)}, silakan lakukan pembayaran di loket.";
        return RedirectToAction(nameof(Index));
    }

    // POST /PesananSaya/Batal/5 : batalkan pesanan sendiri yang belum dibayar sama sekali.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Batal(int id)
    {
        var pembeliId = HttpContext.PembeliId();
        var pesanan = await _db.Pesanan.FirstOrDefaultAsync(p => p.Id == id && p.PembeliId == pembeliId);

        if (pesanan == null)
        {
            return NotFound();
        }

        if (pesanan.Pembayaran > 0)
        {
            TempData["Gagal"] = $"Pesanan {pesanan.IdPesanan} sudah ada pembayaran, pembatalan harus lewat admin.";
            return RedirectToAction(nameof(Index));
        }

        _db.Pesanan.Remove(pesanan);
        await _db.SaveChangesAsync();

        TempData["Sukses"] = $"Pesanan {pesanan.IdPesanan} berhasil dibatalkan.";
        return RedirectToAction(nameof(Index));
    }

    // Isi pilihan kategori (harga + sisa kuota) untuk dropdown.
    private async Task IsiPilihanKategoriAsync()
    {
        var kategori = await _db.KategoriTiket.AsNoTracking().OrderBy(k => k.Id).ToListAsync();
        var pilihan = new List<KategoriPilihan>();
        foreach (var k in kategori)
        {
            pilihan.Add(new KategoriPilihan
            {
                Id = k.Id,
                NamaKategori = k.NamaKategori,
                Harga = k.Harga,
                Sisa = await _db.SisaKuotaAsync(k.Id)
            });
        }
        ViewBag.PilihanKategori = pilihan;
    }
}
