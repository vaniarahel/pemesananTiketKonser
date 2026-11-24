using Microsoft.AspNetCore.Http;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Helpers;

// Fungsi bantu untuk mengecek akun yang sedang login (dipakai di Controller dan View).
public static class Akses
{
    public static string? Peran(this HttpContext context) =>
        context.Session.GetString(WajibLoginAttribute.SessionPeran);

    public static bool IsAdmin(this HttpContext context) =>
        context.Peran() == DaftarPeran.Admin;

    public static bool IsPembeli(this HttpContext context) =>
        context.Peran() == DaftarPeran.Pembeli;

    // Id pembeli milik akun yang login (null kalau bukan akun pembeli).
    public static int? PembeliId(this HttpContext context) =>
        context.Session.GetInt32(WajibLoginAttribute.SessionPembeliId);
}
