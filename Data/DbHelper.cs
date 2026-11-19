using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace PemesananTiketKonser.Data;

// Fungsi bantu yang dipakai beberapa controller (MVC dan API).
public static class DbHelper
{
    // Error MySQL 1062 = data kembar (melanggar unique index).
    public static bool IsDuplicateKey(DbUpdateException ex) =>
        ex.InnerException is MySqlException mysql && mysql.Number == 1062;

    // Error MySQL 1451 = data masih dipakai tabel lain (foreign key restrict).
    public static bool IsForeignKeyViolation(DbUpdateException ex) =>
        ex.InnerException is MySqlException mysql && mysql.Number == 1451;

    // Jumlah tiket yang sudah terjual per kategori: { KategoriId : total JumlahTiket }.
    public static async Task<Dictionary<int, int>> TiketTerjualPerKategoriAsync(this ApplicationDbContext db)
    {
        return await db.Pesanan
            .GroupBy(p => p.KategoriId)
            .Select(g => new { KategoriId = g.Key, Total = g.Sum(p => p.JumlahTiket) })
            .ToDictionaryAsync(x => x.KategoriId, x => x.Total);
    }
}
