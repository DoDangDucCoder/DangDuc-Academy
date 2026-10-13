using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefundRequests",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    IdPurchaseCourse = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AdminNote = table.Column<string>(type: "text", nullable: true),
                    RequestDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: true),
                    ProcessDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRequests", x => x.Id);
                });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 0, 16, 3).PlusNanoseconds(17200700L), new NodaTime.LocalDateTime(2026, 7, 1, 0, 16, 3).PlusNanoseconds(17200700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 0, 16, 3).PlusNanoseconds(17200700L), new NodaTime.LocalDateTime(2026, 7, 1, 0, 16, 3).PlusNanoseconds(17200700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 0, 16, 3).PlusNanoseconds(17200700L), new NodaTime.LocalDateTime(2026, 7, 1, 0, 16, 3).PlusNanoseconds(17200700L) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefundRequests",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 18, 20, 35).PlusNanoseconds(925912100L), new NodaTime.LocalDateTime(2026, 6, 30, 18, 20, 35).PlusNanoseconds(925912100L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 18, 20, 35).PlusNanoseconds(925912100L), new NodaTime.LocalDateTime(2026, 6, 30, 18, 20, 35).PlusNanoseconds(925912100L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 18, 20, 35).PlusNanoseconds(925912100L), new NodaTime.LocalDateTime(2026, 6, 30, 18, 20, 35).PlusNanoseconds(925912100L) });
        }
    }
}
