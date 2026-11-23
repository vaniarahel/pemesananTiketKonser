// pesanan.js
// Menghitung Total Harga dan Status secara langsung di halaman Pesan Tiket / Edit Pesanan,
// tanpa reload. Data harga dan sisa kuota diambil dari atribut data-harga dan data-sisa
// pada pilihan kategori. Ini hanya tampilan perkiraan: server tetap menghitung ulang saat disimpan.
(function () {
    const kategori = document.getElementById("KategoriId");
    const jumlah = document.getElementById("JumlahTiket");
    const bayar = document.getElementById("Pembayaran");
    const total = document.getElementById("totalHarga");
    const status = document.getElementById("statusPesanan");
    const info = document.getElementById("infoKategori");

    // Kalau bukan halaman pesanan, tidak melakukan apa-apa.
    if (!kategori || !jumlah || !bayar || !total || !status || !info) {
        return;
    }

    const maksTiket = parseInt(jumlah.max, 10) || 4;
    const angka = (n) => n.toLocaleString("id-ID");
    const rupiah = (n) => "Rp " + angka(n);

    function hitung() {
        const pilihan = kategori.options[kategori.selectedIndex];
        const harga = parseInt(pilihan?.dataset.harga ?? "0", 10) || 0;
        const sisa = parseInt(pilihan?.dataset.sisa ?? "0", 10) || 0;
        const qty = parseInt(jumlah.value, 10) || 0;
        const uang = parseInt(bayar.value, 10) || 0;

        // ---------- Info harga dan sisa kuota ----------
        let pesanInfo;
        let infoError = false;
        if (harga === 0) {
            pesanInfo = "Pilih kategori untuk melihat harga dan sisa kuota.";
        } else {
            pesanInfo = `Harga satuan ${rupiah(harga)}, sisa kuota ${angka(sisa)} tiket.`;
            if (qty > maksTiket) {
                pesanInfo += ` Maksimal ${maksTiket} tiket per pesanan.`;
                infoError = true;
            } else if (qty > sisa) {
                pesanInfo += sisa <= 0 ? " Kuota sudah habis." : ` Jumlah melebihi sisa kuota.`;
                infoError = true;
            }
        }
        info.textContent = pesanInfo;
        info.classList.toggle("text-danger", infoError);

        // ---------- Total harga dan status ----------
        status.classList.remove("text-success", "text-danger", "fw-semibold");

        if (harga === 0 || qty < 1) {
            total.value = "Dihitung otomatis";
            status.value = "Ditentukan otomatis";
            return;
        }

        const totalHarga = harga * qty;
        total.value = rupiah(totalHarga);

        if (uang > totalHarga) {
            // Kelebihan bayar: tetap LUNAS, tapi tampilkan kembalian.
            status.value = `LUNAS (kembalian ${rupiah(uang - totalHarga)})`;
            status.classList.add("text-success", "fw-semibold");
        } else if (uang === totalHarga) {
            status.value = "LUNAS";
            status.classList.add("text-success", "fw-semibold");
        } else {
            status.value = `BELUM LUNAS (kurang ${rupiah(totalHarga - uang)})`;
            status.classList.add("text-danger", "fw-semibold");
        }
    }

    // Hitung ulang setiap kali kategori, jumlah, atau pembayaran berubah.
    [kategori, jumlah, bayar].forEach((el) => {
        el.addEventListener("input", hitung);
        el.addEventListener("change", hitung);
    });

    // Hitung sekali saat halaman dibuka (berguna di halaman Edit kalau harga kategori sudah berubah).
    hitung();
})();