using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class AddCartAndNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_IdStudent",
                        column: x => x.IdStudent,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseNotes",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    IdContentDetail = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseNotes_ContentCourseDetails_IdContentDetail",
                        column: x => x.IdContentDetail,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "ContentCourseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseNotes_Courses_IdCourse",
                        column: x => x.IdCourse,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseNotes_Users_IdStudent",
                        column: x => x.IdStudent,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCart = table.Column<long>(type: "bigint", nullable: false),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_IdCart",
                        column: x => x.IdCart,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Courses_IdCourse",
                        column: x => x.IdCourse,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 16, 19, 3).PlusNanoseconds(25643100L), new NodaTime.LocalDateTime(2026, 6, 30, 16, 19, 3).PlusNanoseconds(25643100L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 16, 19, 3).PlusNanoseconds(25643100L), new NodaTime.LocalDateTime(2026, 6, 30, 16, 19, 3).PlusNanoseconds(25643100L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 16, 19, 3).PlusNanoseconds(25643100L), new NodaTime.LocalDateTime(2026, 6, 30, 16, 19, 3).PlusNanoseconds(25643100L) });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_IdCart",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "IdCart");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_CourseNotes_IdContentDetail",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "IdContentDetail");

            migrationBuilder.CreateIndex(
                name: "IX_CourseNotes_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_CourseNotes_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "IdStudent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "CourseNotes",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "Carts",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 16, 7, 0, 3).PlusNanoseconds(188571900L), new NodaTime.LocalDateTime(2026, 6, 16, 7, 0, 3).PlusNanoseconds(188571900L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 16, 7, 0, 3).PlusNanoseconds(188571900L), new NodaTime.LocalDateTime(2026, 6, 16, 7, 0, 3).PlusNanoseconds(188571900L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 16, 7, 0, 3).PlusNanoseconds(188571900L), new NodaTime.LocalDateTime(2026, 6, 16, 7, 0, 3).PlusNanoseconds(188571900L) });
        }
    }
}
