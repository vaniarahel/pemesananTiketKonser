using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// Menangani Login dan Logout admin.
public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;

    public AccountController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /Account/Login : tampilkan form login.
    [HttpGet]
    public IActionResult Login()
    {
        // Kalau sudah login, langsung ke Menu Utama.
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString(WajibLoginAttribute.SessionKey)))
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    // POST /Account/Login : cek username dan password ke database.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Password di database disimpan sebagai hash SHA-256, jadi input juga di-hash dulu.
        var hash = HashPassword(model.Password);

        var admin = await _db.Admin
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Username == model.Username && a.Password == hash);

        if (admin == null)
        {
            ModelState.AddModelError(string.Empty, "Username atau password salah.");
            model.Password = "";
            return View(model);
        }

        // Simpan username ke Session sebagai tanda sudah login.
        HttpContext.Session.SetString(WajibLoginAttribute.SessionKey, admin.Username);
        return RedirectToAction("Index", "Home");
    }

    // GET /Account/Logout : hapus Session lalu kembali ke Login.
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    // Mengubah password menjadi hash SHA-256 (64 karakter hex huruf kecil),
    // sama dengan fungsi SHA2('...', 256) di MySQL.
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}