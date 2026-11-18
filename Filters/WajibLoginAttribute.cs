using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PemesananTiketKonser.Filters;

// Pasang [WajibLogin] di atas controller supaya halamannya hanya bisa dibuka setelah login.
// Kalau Session belum berisi username admin, pengguna diarahkan ke halaman Login.
public class WajibLoginAttribute : ActionFilterAttribute
{
    public const string SessionKey = "Admin";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var admin = context.HttpContext.Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(admin))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
    }
}