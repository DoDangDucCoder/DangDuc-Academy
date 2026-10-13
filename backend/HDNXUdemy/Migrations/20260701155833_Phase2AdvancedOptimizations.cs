using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class Phase2AdvancedOptimizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoEngagements",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    IdContentDetail = table.Column<long>(type: "bigint", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    VideoPosition = table.Column<double>(type: "double precision", nullable: false),
                    VideoDuration = table.Column<double>(type: "double precision", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoEngagements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoPlaybackPositions",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    IdContentDetail = table.Column<long>(type: "bigint", nullable: false),
                    PositionSeconds = table.Column<double>(type: "double precision", nullable: false),
                    DurationSeconds = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdated = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoPlaybackPositions", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_VideoEngagement_Content_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "VideoEngagements",
                columns: new[] { "IdContentDetail", "IdStudent" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoPlayback_Student_Content",
                schema: "sche_dev_HDNXUdemy",
                table: "VideoPlaybackPositions",
                columns: new[] { "IdStudent", "IdContentDetail" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoEngagements",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "VideoPlaybackPositions",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 9, 47, 53).PlusNanoseconds(675660700L), new NodaTime.LocalDateTime(2026, 7, 1, 9, 47, 53).PlusNanoseconds(675660700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 9, 47, 53).PlusNanoseconds(675660700L), new NodaTime.LocalDateTime(2026, 7, 1, 9, 47, 53).PlusNanoseconds(675660700L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 9, 47, 53).PlusNanoseconds(675660700L), new NodaTime.LocalDateTime(2026, 7, 1, 9, 47, 53).PlusNanoseconds(675660700L) });
        }
    }
}
