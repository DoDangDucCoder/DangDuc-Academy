namespace HDNXUdemyModel.Model
{
    // ── Exam List/Summary ────────────────────────────────────────────────────
    public class LanguageExamModel
    {
        public int Id { get; set; }
        public string Language { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public string? Level { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int PassScore { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int ViewCount { get; set; }
        public int AttemptCount { get; set; }
    }

    // ── Exam Detail (with parts + questions, no correct answers) ─────────────
    public class LanguageExamDetailModel : LanguageExamModel
    {
        public List<ExamPartModel> Parts { get; set; } = new();
    }

    public class ExamPartModel
    {
        public int Id { get; set; }
        public int PartNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
        public List<ExamQuestionModel> Questions { get; set; } = new();
    }

    public class ExamQuestionModel
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? QuestionImage { get; set; }
        public string? AudioUrl { get; set; }
        public int QuestionType { get; set; }
        public List<AnswerOptionModel> Options { get; set; } = new();
    }

    public class AnswerOptionModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        // IsCorrect NOT exposed here (only in results)
    }

    // ── Start Exam ───────────────────────────────────────────────────────────
    public class StartExamResponse
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }    // StartTime + DurationMinutes
        public int DurationSeconds { get; set; }
        public LanguageExamDetailModel Exam { get; set; } = new();
    }

    // ── Submit Exam ──────────────────────────────────────────────────────────
    public class UserAnswerRequest
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }

    // ── Result ───────────────────────────────────────────────────────────────
    public class ExamResultModel
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public int MaxScore { get; set; }
        public decimal Percentage { get; set; }
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsPassed { get; set; }
        public int TimeTakenSeconds { get; set; }
        public List<QuestionResultModel> QuestionResults { get; set; } = new();
    }

    public class QuestionResultModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int SelectedAnswerId { get; set; }
        public int CorrectAnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
        public List<AnswerOptionWithCorrect> Options { get; set; } = new();
    }

    public class AnswerOptionWithCorrect : AnswerOptionModel
    {
        public bool IsCorrect { get; set; }
    }

    // ── Leaderboard ──────────────────────────────────────────────────────────
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public decimal Percentage { get; set; }
        public int TimeTakenSeconds { get; set; }
        public DateTime SubmitTime { get; set; }
    }

    // ── Vocabulary ───────────────────────────────────────────────────────────
    public class VocabularyModel
    {
        public int Id { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Word { get; set; } = string.Empty;
        public string? Pronunciation { get; set; }
        public string MeaningVi { get; set; } = string.Empty;
        public string? MeaningEn { get; set; }
        public string? ExampleSentence { get; set; }
        public string? ExampleTranslation { get; set; }
        public string? AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? Level { get; set; }
        public string? Tags { get; set; }
        // Progress info (only when fetching review items)
        public DateTime? NextReview { get; set; }
        public bool IsMastered { get; set; }
        public int Repetition { get; set; }
    }

    public class VocabStatsModel
    {
        public string Language { get; set; } = string.Empty;
        public int TotalLearned { get; set; }
        public int MasteredCount { get; set; }
        public int DueForReview { get; set; }
        public int NewToday { get; set; }
        public double ProgressPercent { get; set; }
    }

    // ── Admin Request Models ─────────────────────────────────────────────────
    public class CreateExamRequest
    {
        public string Language { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public string? Level { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; } = 60;
        public int TotalQuestions { get; set; } = 0;
        public int PassScore { get; set; } = 0;
        public string? ThumbnailUrl { get; set; }
    }

    public class CreateQuestionRequest
    {
        public int IdPart { get; set; }
        public int OrderNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? QuestionImage { get; set; }
        public string? AudioUrl { get; set; }
        public string? Explanation { get; set; }
        public int QuestionType { get; set; } = 1;
        public List<CreateAnswerRequest> Answers { get; set; } = new();
    }

    public class CreateAnswerRequest
    {
        public string Label { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class CreateVocabRequest
    {
        public string Language { get; set; } = string.Empty;
        public string Word { get; set; } = string.Empty;
        public string? Pronunciation { get; set; }
        public string MeaningVi { get; set; } = string.Empty;
        public string? MeaningEn { get; set; }
        public string? ExampleSentence { get; set; }
        public string? ExampleTranslation { get; set; }
        public string? AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? Level { get; set; }
        public string? Tags { get; set; }
    }

    public class BulkImportVocabRequest
    {
        public string Language { get; set; } = string.Empty;
        public List<CreateVocabRequest> Items { get; set; } = new();
    }

    public class ExamQuestionAdminModel : ExamQuestionModel
    {
        public List<AnswerOptionWithCorrect> AnswerOptions { get; set; } = new();
    }
}
