using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PemesananTiketKonser.Migrations
{
    /// <inheritdoc />
    public partial class AkunAdminDanPembeli : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "admin",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PembeliId",
                table: "admin",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Peran",
                table: "admin",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PembeliId", "Peran" },
                values: new object[] { null, "Admin" });

            migrationBuilder.UpdateData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Harga", "Kuota" },
                values: new object[] { 2500000, 80 });

            migrationBuilder.UpdateData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 2,
                column: "Harga",
                value: 4000000);

            migrationBuilder.UpdateData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Harga", "Kuota", "NamaKategori" },
                values: new object[] { 2650000, 100, "CAT 1" });

            migrationBuilder.InsertData(
                table: "kategori_tiket",
                columns: new[] { "Id", "Harga", "Kuota", "NamaKategori" },
                values: new object[,]
                {
                    { 4, 1850000, 100, "CAT 2" },
                    { 5, 2350000, 150, "CAT 3" },
                    { 6, 1250000, 150, "CAT 4" },
                    { 7, 850000, 80, "CAT 5" }
                });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "2481016@unai.edu", "Vania Widiyanto" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "2481034@unai.edu", "Josch Matanari" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "2481019@unai.edu", "Ester Manurung" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "2481026@unai.edu", "Jessica Shabrina" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "2481051@unai.edu", "Ivan Halim" });

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Pembayaran", "TotalHarga" },
                values: new object[] { 5000000, 5000000 });

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 2,
                column: "TotalHarga",
                value: 4000000);

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Pembayaran", "TotalHarga" },
                values: new object[] { 10600000, 10600000 });

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 4,
                column: "TotalHarga",
                value: 7500000);

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Pembayaran", "TotalHarga" },
                values: new object[] { 8000000, 8000000 });

            migrationBuilder.CreateIndex(
                name: "IX_admin_PembeliId",
                table: "admin",
                column: "PembeliId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_admin_pembeli_PembeliId",
                table: "admin",
                column: "PembeliId",
                principalTable: "pembeli",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admin_pembeli_PembeliId",
                table: "admin");

            migrationBuilder.DropIndex(
                name: "IX_admin_PembeliId",
                table: "admin");

            migrationBuilder.DeleteData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "PembeliId",
                table: "admin");

            migrationBuilder.DropColumn(
                name: "Peran",
                table: "admin");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "admin",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Harga", "Kuota" },
                values: new object[] { 750000, 200 });

            migrationBuilder.UpdateData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 2,
                column: "Harga",
                value: 1500000);

            migrationBuilder.UpdateData(
                table: "kategori_tiket",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Harga", "Kuota", "NamaKategori" },
                values: new object[] { 500000, 300, "Tribun" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "andi.pratama@example.com", "Andi Pratama" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "bunga.lestari@example.com", "Bunga Lestari" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "citra.maharani@example.com", "Citra Maharani" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "dimas.saputra@example.com", "Dimas Saputra" });

            migrationBuilder.UpdateData(
                table: "pembeli",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Email", "Nama" },
                values: new object[] { "eka.wulandari@example.com", "Eka Wulandari" });

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Pembayaran", "TotalHarga" },
                values: new object[] { 1500000, 1500000 });

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 2,
                column: "TotalHarga",
                value: 1500000);

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Pembayaran", "TotalHarga" },
                values: new object[] { 2000000, 2000000 });

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 4,
                column: "TotalHarga",
                value: 2250000);

            migrationBuilder.UpdateData(
                table: "pesanan",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Pembayaran", "TotalHarga" },
                values: new object[] { 3000000, 3000000 });
        }
    }
}
