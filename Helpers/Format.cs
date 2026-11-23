using System.Globalization;

namespace PemesananTiketKonser.Helpers;

// Fungsi bantu untuk menampilkan angka dengan format Indonesia.
public static class Format
{
    private static readonly CultureInfo Indonesia = new("id-ID");

    // 750000 -> "Rp 750.000"
    public static string Rupiah(int nilai) => "Rp " + nilai.ToString("N0", Indonesia);

    // 1500 -> "1.500"
    public static string Angka(int nilai) => nilai.ToString("N0", Indonesia);
}
