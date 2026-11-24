using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection string MySQL TIDAK ditaruh di appsettings.json (supaya tidak ikut ke Git).
// Isinya diambil dari User Secrets (Visual Studio) atau environment variable (Codespaces).
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Connection string 'Default' belum diisi. Isi lewat klik kanan project > Manage User Secrets.");

// Daftarkan ApplicationDbContext (EF Core + MySQL/MariaDB) supaya bisa dipakai di controller.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllersWithViews();

// Session: menyimpan status login selama 30 menit tanpa aktivitas.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Session harus dipasang setelah UseRouting dan sebelum controller dijalankan.
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// Route untuk halaman MVC: /{controller}/{action}/{id}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Route untuk controller REST API yang memakai [Route("api/...")]
app.MapControllers();

app.Run();