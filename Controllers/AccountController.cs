using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// Menangani Login, Daftar Akun pembeli, dan Logout.
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
        // Kalau sudah login, langsung ke halaman utama sesuai peran.
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString(WajibLoginAttribute.SessionKey)))
        {
            return HalamanUtama(HttpContext.Session.GetString(WajibLoginAttribute.SessionPeran));
        }

        return View(new LoginViewModel());
    }

    // POST /Account/Login : cek username/email dan password ke database.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var input = model.Username.Trim();

        // Admin login pakai username, pembeli pakai email.
        var akun = await _db.Admin
            .AsNoTracking()
            .Include(a => a.Pembeli)
            .FirstOrDefaultAsync(a =>
                a.Username == input || (a.Pembeli != null && a.Pembeli.Email == input));

        // Password dicek di C#, bukan di SQL: MySQL tidak membedakan huruf besar/kecil.
        if (akun != null && !string.Equals(akun.Password, model.Password, StringComparison.Ordinal))
        {
            akun = null;
        }

        if (akun == null)
        {
            ModelState.AddModelError(string.Empty, "Username/email atau password salah.");
            model.Password = "";
            return View(model);
        }

        MasukkanKeSession(akun);
        return HalamanUtama(akun.Peran);
    }

    // GET /Account/Daftar : form daftar akun pembeli.
    [HttpGet]
    public IActionResult Daftar()
    {
        return View(new DaftarViewModel());
    }

    // POST /Account/Daftar : buat data pembeli + akun login sekaligus, lalu langsung login.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Daftar(DaftarViewModel model)
    {
        var email = model.Email.Trim();

        if (await _db.Pembeli.AnyAsync(p => p.Email == email) || await _db.Admin.AnyAsync(a => a.Username == email))
        {
            ModelState.AddModelError(nameof(model.Email),
                "Email sudah terdaftar. Silakan login, atau hubungi admin kalau belum punya password.");
        }

        if (!ModelState.IsValid)
        {
            model.Password = "";
            model.UlangiPassword = "";
            return View(model);
        }

        var akun = new Admin
        {
            Username = email,
            Password = model.Password,
            Peran = DaftarPeran.Pembeli,
            Pembeli = new Pembeli
            {
                Nama = model.Nama.Trim(),
                Email = email,
                NoHp = model.NoHp.Trim(),
                NoIdentitas = model.NoIdentitas.Trim()
            }
        };

        _db.Admin.Add(akun);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (DbHelper.IsDuplicateKey(ex))
        {
            ModelState.AddModelError(nameof(model.Email), "Email sudah terdaftar.");
            model.Password = "";
            model.UlangiPassword = "";
            return View(model);
        }

        akun.PembeliId = akun.Pembeli.Id;
        MasukkanKeSession(akun);
        TempData["Sukses"] = $"Akun berhasil dibuat. Selamat datang, {akun.Pembeli.Nama}!";
        return RedirectToAction("Index", "PesananSaya");
    }

    // GET /Account/Logout : hapus Session lalu kembali ke Login.
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    // Simpan data akun yang login ke Session.
    private void MasukkanKeSession(Admin akun)
    {
        var nama = akun.Peran == DaftarPeran.Pembeli && akun.Pembeli != null ? akun.Pembeli.Nama : akun.Username;
        HttpContext.Session.SetString(WajibLoginAttribute.SessionKey, nama);
        HttpContext.Session.SetString(WajibLoginAttribute.SessionPeran, akun.Peran);

        if (akun.PembeliId.HasValue)
        {
            HttpContext.Session.SetInt32(WajibLoginAttribute.SessionPembeliId, akun.PembeliId.Value);
        }
    }

    // Admin ke Menu Utama, pembeli ke Pesanan Saya.
    private IActionResult HalamanUtama(string? peran) =>
        peran == DaftarPeran.Pembeli
            ? RedirectToAction("Index", "PesananSaya")
            : RedirectToAction("Index", "Home");
}
