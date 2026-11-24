using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Filters;

// Penjaga halaman berdasarkan login dan peran (role).
//
//   [WajibLogin]                       -> semua akun yang sudah login boleh masuk
//   [WajibLogin(DaftarPeran.Admin)]    -> hanya akun Admin
//   [WajibLogin(DaftarPeran.Pembeli)]  -> hanya akun Pembeli
//
// Belum login             -> diarahkan ke halaman Login.
// Login tapi peran salah  -> diarahkan ke halaman utama perannya, dengan pesan "tidak punya akses".
public class WajibLoginAttribute : ActionFilterAttribute
{
    public const string SessionKey = "Admin";          // nama yang tampil di navbar (tanda sudah login)
    public const string SessionPeran = "Peran";        // peran akun yang login
    public const string SessionPembeliId = "PembeliId"; // id pembeli (khusus akun pembeli)

    private readonly string[] _peranDiizinkan;

    public WajibLoginAttribute(params string[] peranDiizinkan)
    {
        _peranDiizinkan = peranDiizinkan;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;

        if (string.IsNullOrEmpty(session.GetString(SessionKey)))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (_peranDiizinkan.Length == 0)
        {
            return;
        }

        var peran = session.GetString(SessionPeran) ?? "";
        if (!_peranDiizinkan.Contains(peran))
        {
            if (context.Controller is Controller controller)
            {
                controller.TempData["Gagal"] = "Akun kamu tidak punya akses ke halaman itu.";
            }

            context.Result = peran == DaftarPeran.Pembeli
                ? new RedirectToActionResult("Index", "PesananSaya", null)
                : new RedirectToActionResult("Index", "Home", null);
        }
    }
}
