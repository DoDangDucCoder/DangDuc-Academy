using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
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
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 9, 20, 59, 33).PlusNanoseconds(552419400L), new NodaTime.LocalDateTime(2026, 6, 9, 20, 59, 33).PlusNanoseconds(552419400L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 9, 20, 59, 33).PlusNanoseconds(552419400L), new NodaTime.LocalDateTime(2026, 6, 9, 20, 59, 33).PlusNanoseconds(552419400L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 9, 20, 59, 33).PlusNanoseconds(552419400L), new NodaTime.LocalDateTime(2026, 6, 9, 20, 59, 33).PlusNanoseconds(552419400L) });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                schema: "sche_dev_HDNXUdemy",
                table: "PasswordResetTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                schema: "sche_dev_HDNXUdemy",
                table: "PasswordResetTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetTokens",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 9, 10, 32, 33).PlusNanoseconds(433935700L), new NodaTime.LocalDateTime(2026, 6, 9, 10, 32, 33).PlusNanoseconds(433935700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 9, 10, 32, 33).PlusNanoseconds(433935700L), new NodaTime.LocalDateTime(2026, 6, 9, 10, 32, 33).PlusNanoseconds(433935700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 9, 10, 32, 33).PlusNanoseconds(433935700L), new NodaTime.LocalDateTime(2026, 6, 9, 10, 32, 33).PlusNanoseconds(433935700L) });
        }
    }
}
