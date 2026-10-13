using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HDNXUdemyData.Migrations
{
    /// <inheritdoc />
    public partial class Phase4_Features_Audit_Search : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: true),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    CertificateCode = table.Column<string>(type: "text", nullable: false),
                    IssuedDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Courses_IdCourse",
                        column: x => x.IdCourse,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_Users_IdStudent",
                        column: x => x.IdStudent,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiveClasses",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    IdInstructor = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    MeetingUrl = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveClasses_Courses_IdCourse",
                        column: x => x.IdCourse,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LiveClasses_Users_IdInstructor",
                        column: x => x.IdInstructor,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCourse = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PassingScore = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Courses_IdCourse",
                        column: x => x.IdCourse,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuiz = table.Column<long>(type: "bigint", nullable: false),
                    IdStudent = table.Column<long>(type: "bigint", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedAt = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Quizzes_IdQuiz",
                        column: x => x.IdQuiz,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Users_IdStudent",
                        column: x => x.IdStudent,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuiz = table.Column<long>(type: "bigint", nullable: false),
                    QuestionText = table.Column<string>(type: "text", nullable: false),
                    Point = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Quizzes_IdQuiz",
                        column: x => x.IdQuiz,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAnswerOptions",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdQuestion = table.Column<long>(type: "bigint", nullable: false),
                    AnswerText = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAnswerOptions_QuizQuestions_IdQuestion",
                        column: x => x.IdQuestion,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttemptAnswers",
                schema: "sche_dev_HDNXUdemy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAttempt = table.Column<long>(type: "bigint", nullable: false),
                    IdQuestion = table.Column<long>(type: "bigint", nullable: false),
                    IdAnswerOption = table.Column<long>(type: "bigint", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateDate = table.Column<LocalDateTime>(type: "timestamp without time zone", nullable: false),
                    CreateBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdateBy = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttemptAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttemptAnswers_QuizAnswerOptions_IdAnswerOption",
                        column: x => x.IdAnswerOption,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "QuizAnswerOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAttemptAnswers_QuizAttempts_IdAttempt",
                        column: x => x.IdAttempt,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "QuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAttemptAnswers_QuizQuestions_IdQuestion",
                        column: x => x.IdQuestion,
                        principalSchema: "sche_dev_HDNXUdemy",
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 12, 15, 20, 55).PlusNanoseconds(341579400L), new NodaTime.LocalDateTime(2026, 6, 12, 15, 20, 55).PlusNanoseconds(341579400L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 12, 15, 20, 55).PlusNanoseconds(341579400L), new NodaTime.LocalDateTime(2026, 6, 12, 15, 20, 55).PlusNanoseconds(341579400L) });

            migrationBuilder.UpdateData(
                schema: "sche_dev_HDNXUdemy",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new NodaTime.LocalDateTime(2026, 6, 12, 15, 20, 55).PlusNanoseconds(341579400L), new NodaTime.LocalDateTime(2026, 6, 12, 15, 20, 55).PlusNanoseconds(341579400L) });

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
                name: "IX_QuizAnswerOptions_IdQuestion",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizAnswerOptions",
                column: "IdQuestion");

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
                name: "IX_QuizQuestions_IdQuiz",
                schema: "sche_dev_HDNXUdemy",
                table: "QuizQuestions",
                column: "IdQuiz");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_IdCourse",
                schema: "sche_dev_HDNXUdemy",
                table: "Quizzes",
                column: "IdCourse");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "Certificates",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "LiveClasses",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "QuizAttemptAnswers",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "QuizAnswerOptions",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "QuizAttempts",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "QuizQuestions",
                schema: "sche_dev_HDNXUdemy");

            migrationBuilder.DropTable(
                name: "Quizzes",
                schema: "sche_dev_HDNXUdemy");

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
        }
    }
}
