using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class AddPayoutSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "RefundStatus",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PayoutRequests",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdInstructor = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PayoutStatus = table.Column<int>(type: "integer", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminNote = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayoutRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayoutRequests_Users_IdInstructor",
                        column: x => x.IdInstructor,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 1, 17, 50).PlusNanoseconds(649649500L), new NodaTime.LocalDateTime(2026, 7, 1, 1, 17, 50).PlusNanoseconds(649649500L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 1, 17, 50).PlusNanoseconds(649649500L), new NodaTime.LocalDateTime(2026, 7, 1, 1, 17, 50).PlusNanoseconds(649649500L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 1, 17, 50).PlusNanoseconds(649649500L), new NodaTime.LocalDateTime(2026, 7, 1, 1, 17, 50).PlusNanoseconds(649649500L) });

            migrationBuilder.CreateIndex(
                name: "IX_PayoutRequests_IdInstructor",
                schema: "sche_dev_HDNXUdemy",
                table: "PayoutRequests",
                column: "IdInstructor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayoutRequests",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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
    }
}
