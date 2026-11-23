using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemesananTiketKonser.Data;
using PemesananTiketKonser.Filters;
using PemesananTiketKonser.Models;

namespace PemesananTiketKonser.Controllers;

// Laporan Penjualan: rekap transaksi + ringkasan per kategori. Wajib login.
[WajibLogin]
public class LaporanController : Controller
{
    private readonly ApplicationDbContext _db;

    public LaporanController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /Laporan?dari=2026-11-01&sampai=2026-11-30&status=LUNAS
    public async Task<IActionResult> Index(DateTime? dari, DateTime? sampai, string? status)
    {
        // Kalau tanggal terbalik, tukar supaya tetap masuk akal.
        if (dari.HasValue && sampai.HasValue && dari > sampai)
        {
            (dari, sampai) = (sampai, dari);
        }

        var query = _db.Pesanan
            .AsNoTracking()
            .Include(p => p.Pembeli)
            .Include(p => p.Kategori)
            .AsQueryable();

        if (dari.HasValue)
        {
            var mulai = dari.Value.Date;
            query = query.Where(p => p.TglPesan >= mulai);
        }

        if (sampai.HasValue)
        {
            // Sampai akhir hari tanggal "sampai".
            var akhir = sampai.Value.Date.AddDays(1);
            query = query.Where(p => p.TglPesan < akhir);
        }

        if (status == Pesanan.StatusLunas || status == Pesanan.StatusBelumLunas)
        {
            query = query.Where(p => p.Status == status);
        }
        else
        {
            status = null;
        }

        var transaksi = await query.OrderBy(p => p.TglPesan).ToListAsync();

        var perKategori = transaksi
            .GroupBy(p => p.Kategori?.NamaKategori ?? "-")
            .Select(g => new LaporanPerKategori
            {
                NamaKategori = g.Key,
                JumlahPesanan = g.Count(),
                TiketTerjual = g.Sum(p => p.JumlahTiket),
                NilaiPenjualan = g.Sum(p => p.TotalHarga),
                UangMasuk = g.Sum(p => Math.Min(p.Pembayaran, p.TotalHarga)),
                Piutang = g.Sum(p => Math.Max(0, p.TotalHarga - p.Pembayaran))
            })
            .OrderByDescending(k => k.NilaiPenjualan)
            .ToList();

        return View(new LaporanViewModel
        {
            Dari = dari,
            Sampai = sampai,
            Status = status,
            Transaksi = transaksi,
            PerKategori = perKategori
        });
    }
}

