using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PemesananTiketKonser.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kategori_tiket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NamaKategori = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Harga = table.Column<int>(type: "int", nullable: false),
                    Kuota = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kategori_tiket", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pembeli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nama = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoHp = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoIdentitas = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pembeli", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pesanan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPesanan = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PembeliId = table.Column<int>(type: "int", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    JumlahTiket = table.Column<int>(type: "int", nullable: false),
                    TotalHarga = table.Column<int>(type: "int", nullable: false),
                    Pembayaran = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TglPesan = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pesanan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pesanan_kategori_tiket_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "kategori_tiket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pesanan_pembeli_PembeliId",
                        column: x => x.PembeliId,
                        principalTable: "pembeli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "admin",
                columns: new[] { "Id", "Password", "Username" },
                values: new object[] { 1, "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", "admin" });

            migrationBuilder.InsertData(
                table: "kategori_tiket",
                columns: new[] { "Id", "Harga", "Kuota", "NamaKategori" },
                values: new object[,]
                {
                    { 1, 750000, 200, "Festival" },
                    { 2, 1500000, 50, "VIP" },
                    { 3, 500000, 300, "Tribun" }
                });

            migrationBuilder.InsertData(
                table: "pembeli",
                columns: new[] { "Id", "Email", "Nama", "NoHp", "NoIdentitas" },
                values: new object[,]
                {
                    { 1, "andi.pratama@example.com", "Andi Pratama", "081211110001", "3273010101010001" },
                    { 2, "bunga.lestari@example.com", "Bunga Lestari", "081211110002", "3273010101010002" },
                    { 3, "citra.maharani@example.com", "Citra Maharani", "081211110003", "3273010101010003" },
                    { 4, "dimas.saputra@example.com", "Dimas Saputra", "081211110004", "3273010101010004" },
                    { 5, "eka.wulandari@example.com", "Eka Wulandari", "081211110005", "3273010101010005" }
                });

            migrationBuilder.InsertData(
                table: "pesanan",
                columns: new[] { "Id", "IdPesanan", "JumlahTiket", "KategoriId", "Pembayaran", "PembeliId", "Status", "TglPesan", "TotalHarga" },
                values: new object[,]
                {
                    { 1, "TK-0001", 2, 1, 1500000, 1, "LUNAS", new DateTime(2026, 11, 10, 10, 15, 0, 0, DateTimeKind.Unspecified), 1500000 },
                    { 2, "TK-0002", 1, 2, 1000000, 2, "BELUM LUNAS", new DateTime(2026, 11, 11, 13, 40, 0, 0, DateTimeKind.Unspecified), 1500000 },
                    { 3, "TK-0003", 4, 3, 2000000, 3, "LUNAS", new DateTime(2026, 11, 12, 9, 5, 0, 0, DateTimeKind.Unspecified), 2000000 },
                    { 4, "TK-0004", 3, 1, 0, 4, "BELUM LUNAS", new DateTime(2026, 11, 14, 16, 20, 0, 0, DateTimeKind.Unspecified), 2250000 },
                    { 5, "TK-0005", 2, 2, 3000000, 1, "LUNAS", new DateTime(2026, 11, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), 3000000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_Username",
                table: "admin",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kategori_tiket_NamaKategori",
                table: "kategori_tiket",
                column: "NamaKategori",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pembeli_Email",
                table: "pembeli",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pesanan_IdPesanan",
                table: "pesanan",
                column: "IdPesanan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pesanan_KategoriId",
                table: "pesanan",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_pesanan_PembeliId",
                table: "pesanan",
                column: "PembeliId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "pesanan");

            migrationBuilder.DropTable(
                name: "kategori_tiket");

            migrationBuilder.DropTable(
                name: "pembeli");
        }
    }
}
