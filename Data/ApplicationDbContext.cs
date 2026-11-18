using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Models;
using System.Reflection.Emit;

namespace PemesananTiketKonser.Data;

// Penghubung aplikasi dengan database MySQL lewat Entity Framework Core.
// Setiap DbSet menjadi satu tabel. Struktur tabel dibuat lewat migrations.
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admin { get; set; } = null!;
    public DbSet<Pembeli> Pembeli { get; set; } = null!;
    public DbSet<KategoriTiket> KategoriTiket { get; set; } = null!;
    public DbSet<Pesanan> Pesanan { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Data yang tidak boleh kembar (UNIQUE) ----------
        modelBuilder.Entity<Admin>().HasIndex(a => a.Username).IsUnique();
        modelBuilder.Entity<Pembeli>().HasIndex(p => p.Email).IsUnique();
        modelBuilder.Entity<KategoriTiket>().HasIndex(k => k.NamaKategori).IsUnique();
        modelBuilder.Entity<Pesanan>().HasIndex(p => p.IdPesanan).IsUnique();

        // ---------- Relasi (FOREIGN KEY) ----------
        // Pembeli / kategori yang sudah punya pesanan tidak bisa dihapus.
        modelBuilder.Entity<Pesanan>()
            .HasOne(p => p.Pembeli)
            .WithMany()
            .HasForeignKey(p => p.PembeliId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pesanan>()
            .HasOne(p => p.Kategori)
            .WithMany()
            .HasForeignKey(p => p.KategoriId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- Data dummy (seeding) ----------
        // Login: username = admin, password = admin123
        // Nilai Password = SHA-256 dari "admin123".
        modelBuilder.Entity<Admin>().HasData(
            new Admin
            {
                Id = 1,
                Username = "admin",
                Password = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9"
            });

        modelBuilder.Entity<Pembeli>().HasData(
            new Pembeli { Id = 1, Nama = "Andi Pratama", Email = "andi.pratama@example.com", NoHp = "081211110001", NoIdentitas = "3273010101010001" },
            new Pembeli { Id = 2, Nama = "Bunga Lestari", Email = "bunga.lestari@example.com", NoHp = "081211110002", NoIdentitas = "3273010101010002" },
            new Pembeli { Id = 3, Nama = "Citra Maharani", Email = "citra.maharani@example.com", NoHp = "081211110003", NoIdentitas = "3273010101010003" },
            new Pembeli { Id = 4, Nama = "Dimas Saputra", Email = "dimas.saputra@example.com", NoHp = "081211110004", NoIdentitas = "3273010101010004" },
            new Pembeli { Id = 5, Nama = "Eka Wulandari", Email = "eka.wulandari@example.com", NoHp = "081211110005", NoIdentitas = "3273010101010005" });

        modelBuilder.Entity<KategoriTiket>().HasData(
            new KategoriTiket { Id = 1, NamaKategori = "Festival", Harga = 750000, Kuota = 200 },
            new KategoriTiket { Id = 2, NamaKategori = "VIP", Harga = 1500000, Kuota = 50 },
            new KategoriTiket { Id = 3, NamaKategori = "Tribun", Harga = 500000, Kuota = 300 });

        // TotalHarga = harga x jumlah; Status = LUNAS kalau Pembayaran >= TotalHarga.
        // (ditulis "Models.Pesanan" karena di class ini "Pesanan" juga nama DbSet)
        modelBuilder.Entity<Pesanan>().HasData(
            new Pesanan { Id = 1, IdPesanan = "TK-0001", PembeliId = 1, KategoriId = 1, JumlahTiket = 2, TotalHarga = 1500000, Pembayaran = 1500000, Status = Models.Pesanan.StatusLunas, TglPesan = new DateTime(2026, 11, 10, 10, 15, 0) },
            new Pesanan { Id = 2, IdPesanan = "TK-0002", PembeliId = 2, KategoriId = 2, JumlahTiket = 1, TotalHarga = 1500000, Pembayaran = 1000000, Status = Models.Pesanan.StatusBelumLunas, TglPesan = new DateTime(2026, 11, 11, 13, 40, 0) },
            new Pesanan { Id = 3, IdPesanan = "TK-0003", PembeliId = 3, KategoriId = 3, JumlahTiket = 4, TotalHarga = 2000000, Pembayaran = 2000000, Status = Models.Pesanan.StatusLunas, TglPesan = new DateTime(2026, 11, 12, 9, 5, 0) },
            new Pesanan { Id = 4, IdPesanan = "TK-0004", PembeliId = 4, KategoriId = 1, JumlahTiket = 3, TotalHarga = 2250000, Pembayaran = 0, Status = Models.Pesanan.StatusBelumLunas, TglPesan = new DateTime(2026, 11, 14, 16, 20, 0) },
            new Pesanan { Id = 5, IdPesanan = "TK-0005", PembeliId = 1, KategoriId = 2, JumlahTiket = 2, TotalHarga = 3000000, Pembayaran = 3000000, Status = Models.Pesanan.StatusLunas, TglPesan = new DateTime(2026, 11, 15, 11, 0, 0) });
    }
}