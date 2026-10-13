using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class Phase1_AddRefreshTokens_FKConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "sche_dev_HDNXUdemy",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TheadQuestionCourses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_TheadQuestionCourses_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "SubCategories",
                column: "IdCategory");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPromotions_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPromotions_IdPromotion",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions",
                column: "IdPromotion");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPromotions_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProcess_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_PurcharseCourses_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_PurcharseCourses_PurcharseCode",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses",
                column: "PurcharseCode",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_PurcharseCourses_Amounts_NonNegative",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses",
                sql: "\"TotalPrice\" >= 0 AND \"DiscountAmount\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_PurcharseCourseDetails_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_PurcharseCourseDetails_IdPurchaseOrder",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                column: "IdPurchaseOrder");

            migrationBuilder.CreateIndex(
                name: "IX_PurcharseCourseDetails_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                column: "IdStudent");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PurcharseCourseDetails_Prices_NonNegative",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                sql: "\"PriceOfCourse\" >= 0 AND \"PriceOfDiscount\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCodes_StripePromotionCodeId",
                schema: "sche_dev_HDNXUdemy",
                table: "PromotionCodes",
                column: "StripePromotionCodeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_DetailTheadQuestionCourses_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_DetailTheadQuestionCourses_IdTheadQuestionCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses",
                column: "IdTheadQuestionCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "Courses",
                column: "IdCategory");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Courses_Prices_NonNegative",
                schema: "sche_dev_HDNXUdemy",
                table: "Courses",
                sql: "\"PriceOfCourse\" >= 0 AND \"PriceOfDiscount\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEvaluations_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEvaluations_IdStudent_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations",
                columns: new[] { "IdStudent", "IdCourse" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_StripeCouponId",
                schema: "sche_dev_HDNXUdemy",
                table: "Coupons",
                column: "StripeCouponId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentCourses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourses",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_ContentCourseDetails_IdContent",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourseDetails",
                column: "IdContent");

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkCourses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkCourses_IdStudent_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses",
                columns: new[] { "IdStudent", "IdCourse" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "sche_dev_HDNXUdemy",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "sche_dev_HDNXUdemy",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookmarkCourses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookmarkCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentCourseDetails_ContentCourses_IdContent",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourseDetails",
                column: "IdContent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "ContentCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentCourses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourses",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEvaluations_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEvaluations_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Categories_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "Courses",
                column: "IdCategory",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetailTheadQuestionCourses_TheadQuestionCourses_IdTheadQues~",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses",
                column: "IdTheadQuestionCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "TheadQuestionCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetailTheadQuestionCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurcharseCourseDetails_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurcharseCourseDetails_PurcharseCourses_IdPurchaseOrder",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                column: "IdPurchaseOrder",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "PurcharseCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurcharseCourseDetails_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurcharseCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProcess_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPromotions_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPromotions_PromotionCodes_IdPromotion",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions",
                column: "IdPromotion",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "PromotionCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPromotions_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "SubCategories",
                column: "IdCategory",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TheadQuestionCourses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TheadQuestionCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookmarkCourses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_BookmarkCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentCourseDetails_ContentCourses_IdContent",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentCourses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEvaluations_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEvaluations_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Categories_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailTheadQuestionCourses_TheadQuestionCourses_IdTheadQues~",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailTheadQuestionCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PurcharseCourseDetails_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurcharseCourseDetails_PurcharseCourses_IdPurchaseOrder",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurcharseCourseDetails_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurcharseCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProcess_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPromotions_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPromotions_PromotionCodes_IdPromotion",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentPromotions_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "SubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_TheadQuestionCourses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_TheadQuestionCourses_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "sche_dev_HDNXUdemy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TheadQuestionCourses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses");

            migrationBuilder.DropIndex(
                name: "IX_TheadQuestionCourses_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "TheadQuestionCourses");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_StudentPromotions_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions");

            migrationBuilder.DropIndex(
                name: "IX_StudentPromotions_IdPromotion",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions");

            migrationBuilder.DropIndex(
                name: "IX_StudentPromotions_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentPromotions");

            migrationBuilder.DropIndex(
                name: "IX_StudentProcess_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "StudentProcess");

            migrationBuilder.DropIndex(
                name: "IX_PurcharseCourses_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses");

            migrationBuilder.DropIndex(
                name: "IX_PurcharseCourses_PurcharseCode",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PurcharseCourses_Amounts_NonNegative",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourses");

            migrationBuilder.DropIndex(
                name: "IX_PurcharseCourseDetails_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurcharseCourseDetails_IdPurchaseOrder",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurcharseCourseDetails_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PurcharseCourseDetails_Prices_NonNegative",
                schema: "sche_dev_HDNXUdemy",
                table: "PurcharseCourseDetails");

            migrationBuilder.DropIndex(
                name: "IX_PromotionCodes_StripePromotionCodeId",
                schema: "sche_dev_HDNXUdemy",
                table: "PromotionCodes");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_DetailTheadQuestionCourses_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses");

            migrationBuilder.DropIndex(
                name: "IX_DetailTheadQuestionCourses_IdTheadQuestionCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "DetailTheadQuestionCourses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_IdCategory",
                schema: "sche_dev_HDNXUdemy",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Courses_Prices_NonNegative",
                schema: "sche_dev_HDNXUdemy",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_CourseEvaluations_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_CourseEvaluations_IdStudent_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_StripeCouponId",
                schema: "sche_dev_HDNXUdemy",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_ContentCourses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourses");

            migrationBuilder.DropIndex(
                name: "IX_ContentCourseDetails_IdContent",
                schema: "sche_dev_HDNXUdemy",
                table: "ContentCourseDetails");

            migrationBuilder.DropIndex(
                name: "IX_BookmarkCourses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses");

            migrationBuilder.DropIndex(
                name: "IX_BookmarkCourses_IdStudent_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "BookmarkCourses");

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2024, 3, 26, 11, 46, 13).PlusNanoseconds(508208800L), new NodaTime.LocalDateTime(2024, 3, 26, 11, 46, 13).PlusNanoseconds(508208800L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2024, 3, 26, 11, 46, 13).PlusNanoseconds(508208800L), new NodaTime.LocalDateTime(2024, 3, 26, 11, 46, 13).PlusNanoseconds(508208800L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2024, 3, 26, 11, 46, 13).PlusNanoseconds(508208800L), new NodaTime.LocalDateTime(2024, 3, 26, 11, 46, 13).PlusNanoseconds(508208800L) });
        }
    }
}
