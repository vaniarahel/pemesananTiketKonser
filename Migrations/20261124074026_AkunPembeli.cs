using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PemesananTiketKonser.Migrations
{
    /// <inheritdoc />
    public partial class AkunPembeli : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "admin123");

            migrationBuilder.InsertData(
                table: "admin",
                columns: new[] { "Id", "Password", "PembeliId", "Peran", "Username" },
                values: new object[,]
                {
                    { 2, "vania123", 1, "Pembeli", "2481016@unai.edu" },
                    { 3, "josch123", 2, "Pembeli", "2481034@unai.edu" },
                    { 4, "ester123", 3, "Pembeli", "2481019@unai.edu" },
                    { 5, "jessica123", 4, "Pembeli", "2481026@unai.edu" },
                    { 6, "ivan123", 5, "Pembeli", "2481051@unai.edu" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "admin",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9");
        }
    }
}
