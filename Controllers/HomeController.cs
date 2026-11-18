using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    // ApplicationDbContext dikirim otomatis oleh ASP.NET Core (dependency injection).
    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Hanya bisa dibuka setelah login. (Halaman Error sengaja tidak dikunci.)
    [WajibLogin]
    public async Task<IActionResult> Index()
    {
        // TES KONEKSI SEMENTARA: hitung jumlah kategori tiket lewat EF Core.
        // Bagian ini nanti diganti dengan isi Menu Utama (dashboard).
        try
        {
            ViewBag.JumlahKategori = await _db.KategoriTiket.CountAsync();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorDb = ex.Message;
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}