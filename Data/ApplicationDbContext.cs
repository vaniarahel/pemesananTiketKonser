using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Models;

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
        // Satu data pembeli hanya boleh punya satu akun login.
        modelBuilder.Entity<Admin>().HasIndex(a => a.PembeliId).IsUnique();

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

        // Akun login pembeli terhubung ke satu data pembeli.
        // Pembeli yang punya akun tidak bisa dihapus.
        modelBuilder.Entity<Admin>()
            .HasOne(a => a.Pembeli)
            .WithMany()
            .HasForeignKey(a => a.PembeliId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- Data dummy (seeding) ----------
        // Akun login (password disimpan apa adanya).
        // Admin  : username = admin, password = admin123
        // Pembeli: username = email masing-masing, password = nama depan + 123
        // Pembeli baru bisa membuat akun sendiri lewat halaman Daftar.
        modelBuilder.Entity<Admin>().HasData(
            new Admin { Id = 1, Username = "admin", Password = "admin123", Peran = DaftarPeran.Admin },
            new Admin { Id = 2, Username = "2481016@unai.edu", Password = "vania123", Peran = DaftarPeran.Pembeli, PembeliId = 1 },
            new Admin { Id = 3, Username = "2481034@unai.edu", Password = "josch123", Peran = DaftarPeran.Pembeli, PembeliId = 2 },
            new Admin { Id = 4, Username = "2481019@unai.edu", Password = "ester123", Peran = DaftarPeran.Pembeli, PembeliId = 3 },
            new Admin { Id = 5, Username = "2481026@unai.edu", Password = "jessica123", Peran = DaftarPeran.Pembeli, PembeliId = 4 },
            new Admin { Id = 6, Username = "2481051@unai.edu", Password = "ivan123", Peran = DaftarPeran.Pembeli, PembeliId = 5 });

        modelBuilder.Entity<Pembeli>().HasData(
            new Pembeli { Id = 1, Nama = "Vania Widiyanto", Email = "2481016@unai.edu", NoHp = "081211110001", NoIdentitas = "3273010101010001" },
            new Pembeli { Id = 2, Nama = "Josch Matanari", Email = "2481034@unai.edu", NoHp = "081211110002", NoIdentitas = "3273010101010002" },
            new Pembeli { Id = 3, Nama = "Ester Manurung", Email = "2481019@unai.edu", NoHp = "081211110003", NoIdentitas = "3273010101010003" },
            new Pembeli { Id = 4, Nama = "Jessica Shabrina", Email = "2481026@unai.edu", NoHp = "081211110004", NoIdentitas = "3273010101010004" },
            new Pembeli { Id = 5, Nama = "Ivan Halim", Email = "2481051@unai.edu", NoHp = "081211110005", NoIdentitas = "3273010101010005" });

        modelBuilder.Entity<KategoriTiket>().HasData(
            new KategoriTiket { Id = 1, NamaKategori = "Festival", Harga = 2500000, Kuota = 80 },
            new KategoriTiket { Id = 2, NamaKategori = "VIP", Harga = 4000000, Kuota = 50 },
            new KategoriTiket { Id = 3, NamaKategori = "CAT 1", Harga = 2650000, Kuota = 100 },
            new KategoriTiket { Id = 4, NamaKategori = "CAT 2", Harga = 1850000, Kuota = 100 },
            new KategoriTiket { Id = 5, NamaKategori = "CAT 3", Harga = 2350000, Kuota = 150 },
            new KategoriTiket { Id = 6, NamaKategori = "CAT 4", Harga = 1250000, Kuota = 150 },
            new KategoriTiket { Id = 7, NamaKategori = "CAT 5", Harga = 850000, Kuota = 80 });

        // TotalHarga = harga x jumlah; Status = LUNAS kalau Pembayaran >= TotalHarga.
        // (ditulis "Models.Pesanan" karena di class ini "Pesanan" juga nama DbSet)
        modelBuilder.Entity<Pesanan>().HasData(
            // Festival 2.500.000 x 2
            new Pesanan { Id = 1, IdPesanan = "TK-0001", PembeliId = 1, KategoriId = 1, JumlahTiket = 2, TotalHarga = 5000000, Pembayaran = 5000000, Status = Models.Pesanan.StatusLunas, TglPesan = new DateTime(2026, 11, 10, 10, 15, 0) },
            // VIP 4.000.000 x 1 (baru bayar 1 juta)
            new Pesanan { Id = 2, IdPesanan = "TK-0002", PembeliId = 2, KategoriId = 2, JumlahTiket = 1, TotalHarga = 4000000, Pembayaran = 1000000, Status = Models.Pesanan.StatusBelumLunas, TglPesan = new DateTime(2026, 11, 11, 13, 40, 0) },
            // CAT 1 2.650.000 x 4
            new Pesanan { Id = 3, IdPesanan = "TK-0003", PembeliId = 3, KategoriId = 3, JumlahTiket = 4, TotalHarga = 10600000, Pembayaran = 10600000, Status = Models.Pesanan.StatusLunas, TglPesan = new DateTime(2026, 11, 12, 9, 5, 0) },
            // Festival 2.500.000 x 3 (belum bayar)
            new Pesanan { Id = 4, IdPesanan = "TK-0004", PembeliId = 4, KategoriId = 1, JumlahTiket = 3, TotalHarga = 7500000, Pembayaran = 0, Status = Models.Pesanan.StatusBelumLunas, TglPesan = new DateTime(2026, 11, 14, 16, 20, 0) },
            // VIP 4.000.000 x 2
            new Pesanan { Id = 5, IdPesanan = "TK-0005", PembeliId = 1, KategoriId = 2, JumlahTiket = 2, TotalHarga = 8000000, Pembayaran = 8000000, Status = Models.Pesanan.StatusLunas, TglPesan = new DateTime(2026, 11, 15, 11, 0, 0) });
    }
}