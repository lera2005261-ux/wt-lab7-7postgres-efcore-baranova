using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace wt_lab7_7postgres_efcore_baranova.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Компьютеры", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Ноутбук", 2500m },
                    { 2, "Аксессуары", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), true, "Мышь", 80m },
                    { 3, "Аксессуары", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), false, "Клавиатура", 150m },
                    { 4, "Компьютеры", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, "Монитор", 900m },
                    { 5, "Аудио", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, "Наушники", 320m },
                    { 6, "Телефоны", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, "Смартфон", 1800m }
                });
            migrationBuilder.Sql(
    @"SELECT setval(pg_get_serial_sequence('""Products""', 'Id'), (SELECT MAX(""Id"") FROM ""Products""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
