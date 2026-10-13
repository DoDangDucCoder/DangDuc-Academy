using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class Phase4GamificationLearningPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementBadges",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IconEmoji = table.Column<string>(type: "text", nullable: false),
                    CriteriaType = table.Column<int>(type: "integer", nullable: false),
                    CriteriaValue = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementBadges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisites",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    IdPrerequisiteCourse = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningPathCourses",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLearningPath = table.Column<long>(type: "bigint", nullable: false),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPathCourses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningPaths",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IdCategory = table.Column<long>(type: "bigint", nullable: false),
                    PictureUrl = table.Column<string>(type: "text", nullable: true),
                    LevelRequired = table.Column<string>(type: "text", nullable: false),
                    EstimatedHours = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPaths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningStreaks",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    CurrentStreak = table.Column<int>(type: "integer", nullable: false),
                    LongestStreak = table.Column<int>(type: "integer", nullable: false),
                    LastActivityDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    TotalDaysLearned = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningStreaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBadges",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    IdBadge = table.Column<long>(type: "bigint", nullable: false),
                    EarnedDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBadges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XPTransactions",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    IdCourse = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XPTransactions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 16, 34, 14).PlusNanoseconds(695794900L), new NodaTime.LocalDateTime(2026, 7, 1, 16, 34, 14).PlusNanoseconds(695794900L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 16, 34, 14).PlusNanoseconds(695794900L), new NodaTime.LocalDateTime(2026, 7, 1, 16, 34, 14).PlusNanoseconds(695794900L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 16, 34, 14).PlusNanoseconds(695794900L), new NodaTime.LocalDateTime(2026, 7, 1, 16, 34, 14).PlusNanoseconds(695794900L) });

            migrationBuilder.CreateIndex(
                name: "IX_Prerequisite_Course",
                schema: "sche_dev_HDNXUdemy",
                table: "CoursePrerequisites",
                columns: new[] { "IdCourse", "IdPrerequisiteCourse" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourse_Path_Course",
                schema: "sche_dev_HDNXUdemy",
                table: "LearningPathCourses",
                columns: new[] { "IdLearningPath", "IdCourse" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningPath_Category",
                schema: "sche_dev_HDNXUdemy",
                table: "LearningPaths",
                column: "IdCategory");

            migrationBuilder.CreateIndex(
                name: "IX_LearningStreak_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "LearningStreaks",
                column: "IdStudent",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBadge_Student_Badge",
                schema: "sche_dev_HDNXUdemy",
                table: "UserBadges",
                columns: new[] { "IdStudent", "IdBadge" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_XPTransaction_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "XPTransactions",
                column: "IdStudent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementBadges",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "CoursePrerequisites",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "LearningPathCourses",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "LearningPaths",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "LearningStreaks",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "UserBadges",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "XPTransactions",
                schema: "sche_dev_HDNXUdemy");

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
        }
    }
}
