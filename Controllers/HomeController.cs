using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Helpers;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET / : Menu Utama (dashboard). Hanya bisa dibuka setelah login.
    [WajibLogin]
    public async Task<IActionResult> Index()
    {
        // Pembeli tidak punya dashboard, langsung ke Pesanan Saya.
        if (!HttpContext.IsAdmin())
        {
            return RedirectToAction("Index", "PesananSaya");
        }

        var pesanan = await _db.Pesanan.AsNoTracking().ToListAsync();
        var kategori = await _db.KategoriTiket.AsNoTracking().OrderBy(k => k.Id).ToListAsync();
        var terjual = await _db.TiketTerjualPerKategoriAsync();

        var model = new DashboardViewModel
        {
            TotalPembeli = await _db.Pembeli.CountAsync(),
            TotalPesanan = pesanan.Count,
            TotalTiket = pesanan.Sum(p => p.JumlahTiket),
            PesananBelumLunas = pesanan.Count(p => p.Status == Pesanan.StatusBelumLunas),
            TotalPendapatan = pesanan.Sum(p => Math.Min(p.Pembayaran, p.TotalHarga)),
            BelumDibayar = pesanan.Sum(p => Math.Max(0, p.TotalHarga - p.Pembayaran)),
            Kuota = kategori.Select(k => new KategoriDenganSisa
            {
                Kategori = k,
                Terjual = terjual.GetValueOrDefault(k.Id)
            }).ToList(),
            PesananTerbaru = await _db.Pesanan
                .AsNoTracking()
                .Include(p => p.Pembeli)
                .Include(p => p.Kategori)
                .OrderByDescending(p => p.TglPesan)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Halaman error sengaja tidak dikunci login.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}