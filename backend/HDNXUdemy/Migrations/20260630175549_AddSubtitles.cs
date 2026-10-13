using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class AddSubtitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_IdCart",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseNotes_ContentCourseDetails_IdContentDetail",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseNotes_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseNotes_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_LiveClasses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_LiveClasses_Users_IdInstructor",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswerOptions_QuizQuestions_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttemptAnswers_QuizAnswerOptions_IdAnswerOption",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttemptAnswers_QuizAttempts_IdAttempt",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttemptAnswers_QuizQuestions_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Quizzes_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Quizzes_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Quizzes");

            migrationBuilder.DropTable(
                name: "Articles",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttemptAnswers_IdAnswerOption",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttemptAnswers_IdAttempt",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttemptAnswers_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswerOptions_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAnswerOptions");

            migrationBuilder.DropIndex(
                name: "IX_LiveClasses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses");

            migrationBuilder.DropIndex(
                name: "IX_LiveClasses_IdInstructor",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses");

            migrationBuilder.DropIndex(
                name: "IX_CourseNotes_IdContentDetail",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropIndex(
                name: "IX_CourseNotes_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropIndex(
                name: "IX_CourseNotes_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_CertificateCode",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_IdStudent_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Carts_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_IdCart",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.AddColumn<long>(
                name: "ContentDetailId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CartId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Subtitles",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdContentDetail = table.Column<long>(type: "bigint", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subtitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subtitles_ContentCourseDetails_IdContentDetail",
                        column: x => x.IdContentDetail,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "ContentCourseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 17, 55, 48).PlusNanoseconds(734887000L), new NodaTime.LocalDateTime(2026, 6, 30, 17, 55, 48).PlusNanoseconds(734887000L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 17, 55, 48).PlusNanoseconds(734887000L), new NodaTime.LocalDateTime(2026, 6, 30, 17, 55, 48).PlusNanoseconds(734887000L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 30, 17, 55, 48).PlusNanoseconds(734887000L), new NodaTime.LocalDateTime(2026, 6, 30, 17, 55, 48).PlusNanoseconds(734887000L) });

            migrationBuilder.CreateIndex(
                name: "IX_CourseNotes_ContentDetailId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "ContentDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseNotes_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseNotes_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_IdContentDetail",
                schema: "sche_dev_HDNXUdemy",
                table: "Subtitles",
                column: "IdContentDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "CartId",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Courses_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "CourseId",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts",
                column: "StudentId",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseNotes_ContentCourseDetails_ContentDetailId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "ContentDetailId",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "ContentCourseDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseNotes_Courses_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "CourseId",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseNotes_Users_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "StudentId",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Courses_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseNotes_ContentCourseDetails_ContentDetailId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseNotes_Courses_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseNotes_Users_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropTable(
                name: "Subtitles",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropIndex(
                name: "IX_CourseNotes_ContentDetailId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropIndex(
                name: "IX_CourseNotes_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropIndex(
                name: "IX_CourseNotes_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropIndex(
                name: "IX_Carts_StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ContentDetailId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropColumn(
                name: "CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CartId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CourseId",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems");

            migrationBuilder.CreateTable(
                name: "Articles",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    ViewCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: true),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
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
                name: "IX_Quizzes_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Quizzes",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizQuestions",
                column: "IdQuiz");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts",
                column: "IdQuiz");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts",
                column: "IdStudent");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttemptAnswers_IdAnswerOption",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers",
                column: "IdAnswerOption");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttemptAnswers_IdAttempt",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers",
                column: "IdAttempt");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttemptAnswers_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswerOptions_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAnswerOptions",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_LiveClasses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_LiveClasses_IdInstructor",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses",
                column: "IdInstructor");

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

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateCode",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates",
                column: "CertificateCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_IdStudent_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates",
                columns: new[] { "IdStudent", "IdCourse" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts",
                column: "IdStudent");

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
                name: "IX_Articles_AuthorId",
                schema: "sche_dev_HDNXUdemy",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Slug",
                schema: "sche_dev_HDNXUdemy",
                table: "Articles",
                column: "Slug",
                unique: true);

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
                name: "FK_CartItems_Carts_IdCart",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "IdCart",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CartItems",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Carts",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "Certificates",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseNotes_ContentCourseDetails_IdContentDetail",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "IdContentDetail",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "ContentCourseDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseNotes_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseNotes_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "CourseNotes",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LiveClasses_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LiveClasses_Users_IdInstructor",
                schema: "sche_dev_HDNXUdemy",
                table: "LiveClasses",
                column: "IdInstructor",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswerOptions_QuizQuestions_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAnswerOptions",
                column: "IdQuestion",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "QuizQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttemptAnswers_QuizAnswerOptions_IdAnswerOption",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers",
                column: "IdAnswerOption",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "QuizAnswerOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttemptAnswers_QuizAttempts_IdAttempt",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers",
                column: "IdAttempt",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "QuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttemptAnswers_QuizQuestions_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttemptAnswers",
                column: "IdQuestion",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "QuizQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Quizzes_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts",
                column: "IdQuiz",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Users_IdStudent",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAttempts",
                column: "IdStudent",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Quizzes_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizQuestions",
                column: "IdQuiz",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Quizzes",
                column: "IdCourse",
                principalSchema: "sche_dev_HDNXUdemy",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
