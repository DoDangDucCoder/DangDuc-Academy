using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class Phase3UdemyFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Announcements",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    IdInstructor = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonCompletions",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    IdContentDetail = table.Column<long>(type: "bigint", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonCompletions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QAVotes",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestion = table.Column<long>(type: "bigint", nullable: false),
                    IdUser = table.Column<long>(type: "bigint", nullable: false),
                    IsUpvote = table.Column<bool>(type: "boolean", nullable: false),
                    IsForAnswer = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QAVotes", x => x.Id);
                });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 16, 17, 43).PlusNanoseconds(827934700L), new NodaTime.LocalDateTime(2026, 7, 1, 16, 17, 43).PlusNanoseconds(827934700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 16, 17, 43).PlusNanoseconds(827934700L), new NodaTime.LocalDateTime(2026, 7, 1, 16, 17, 43).PlusNanoseconds(827934700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 16, 17, 43).PlusNanoseconds(827934700L), new NodaTime.LocalDateTime(2026, 7, 1, 16, 17, 43).PlusNanoseconds(827934700L) });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_Course",
                schema: "sche_dev_HDNXUdemy",
                table: "Announcements",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_LessonCompletion_Student_Content",
                schema: "sche_dev_HDNXUdemy",
                table: "LessonCompletions",
                columns: new[] { "IdStudent", "IdContentDetail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonCompletion_Student_Course",
                schema: "sche_dev_HDNXUdemy",
                table: "LessonCompletions",
                columns: new[] { "IdStudent", "IdCourse" });

            migrationBuilder.CreateIndex(
                name: "IX_QAVotes_Question_User",
                schema: "sche_dev_HDNXUdemy",
                table: "QAVotes",
                columns: new[] { "IdQuestion", "IdUser", "IsForAnswer" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "LessonCompletions",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "QAVotes",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 15, 58, 33).PlusNanoseconds(109356300L), new NodaTime.LocalDateTime(2026, 7, 1, 15, 58, 33).PlusNanoseconds(109356300L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 15, 58, 33).PlusNanoseconds(109356300L), new NodaTime.LocalDateTime(2026, 7, 1, 15, 58, 33).PlusNanoseconds(109356300L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 15, 58, 33).PlusNanoseconds(109356300L), new NodaTime.LocalDateTime(2026, 7, 1, 15, 58, 33).PlusNanoseconds(109356300L) });
        }
    }
}
