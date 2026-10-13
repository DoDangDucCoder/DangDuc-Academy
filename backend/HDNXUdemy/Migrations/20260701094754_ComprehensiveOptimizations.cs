using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class ComprehensiveOptimizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentProcess_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess");

            migrationBuilder.RenameIndex(
                name: "IX_Subtitles_IdContentDetail",
                schema: "sche_dev_HDNXUdemy",
                table: "Subtitles",
                newName: "IX_Subtitles_ContentDetail");

            migrationBuilder.RenameIndex(
                name: "IX_PurcharseCourseDetails_IdPurchaseOrder",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                newName: "IX_PurchaseDetails_PurchaseOrder");

            migrationBuilder.RenameIndex(
                name: "IX_PayoutRequests_IdInstructor",
                schema: "sche_dev_HDNXUdemy",
                table: "PayoutRequests",
                newName: "IX_PayoutRequests_Instructor");

            migrationBuilder.CreateTable(
                name: "InstructorPaymentMethods",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdInstructor = table.Column<long>(type: "bigint", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    AccountHolderName = table.Column<string>(type: "text", nullable: false),
                    PaypalEmail = table.Column<string>(type: "text", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    InstructorId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorPaymentMethods_Users_IdInstructor",
                        column: x => x.IdInstructor,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstructorPaymentMethods_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_StudentProcess_Course_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess",
                columns: new[] { "IdCourse", "CreateBy" });

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_Status",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests",
                column: "RefundStatus");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_PayoutRequests_Status",
                schema: "sche_dev_HDNXUdemy",
                table: "PayoutRequests",
                column: "PayoutStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CourseNotes_Course_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                columns: new[] { "IdCourse", "IdStudent" });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorPaymentMethods_Instructor",
                schema: "sche_dev_HDNXUdemy",
                table: "InstructorPaymentMethods",
                column: "IdInstructor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstructorPaymentMethods_InstructorId",
                schema: "sche_dev_HDNXUdemy",
                table: "InstructorPaymentMethods",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorPaymentMethods",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropIndex(
                name: "IX_StudentProcess_Course_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess");

            migrationBuilder.DropIndex(
                name: "IX_RefundRequests_Status",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests");

            migrationBuilder.DropIndex(
                name: "IX_RefundRequests_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "RefundRequests");

            migrationBuilder.DropIndex(
                name: "IX_PayoutRequests_Status",
                schema: "sche_dev_HDNXUdemy",
                table: "PayoutRequests");

            migrationBuilder.DropIndex(
                name: "IX_CourseNotes_Course_Student",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.RenameIndex(
                name: "IX_Subtitles_ContentDetail",
                schema: "sche_dev_HDNXUdemy",
                table: "Subtitles",
                newName: "IX_Subtitles_IdContentDetail");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseDetails_PurchaseOrder",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                newName: "IX_PurcharseCourseDetails_IdPurchaseOrder");

            migrationBuilder.RenameIndex(
                name: "IX_PayoutRequests_Instructor",
                schema: "sche_dev_HDNXUdemy",
                table: "PayoutRequests",
                newName: "IX_PayoutRequests_IdInstructor");

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 8, 57, 49).PlusNanoseconds(433855900L), new NodaTime.LocalDateTime(2026, 7, 1, 8, 57, 49).PlusNanoseconds(433855900L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 8, 57, 49).PlusNanoseconds(433855900L), new NodaTime.LocalDateTime(2026, 7, 1, 8, 57, 49).PlusNanoseconds(433855900L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 7, 1, 8, 57, 49).PlusNanoseconds(433855900L), new NodaTime.LocalDateTime(2026, 7, 1, 8, 57, 49).PlusNanoseconds(433855900L) });

            migrationBuilder.CreateIndex(
                name: "IX_StudentProcess_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess",
                column: "IdCourse");
        }
    }
}
