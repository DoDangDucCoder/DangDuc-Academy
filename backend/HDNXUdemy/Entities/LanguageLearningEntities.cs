using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    [Table("LanguageExams", Schema = "sche_dev_HDNXUdemy")]
    public class LanguageExamEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(5)]  public string Language { get; set; } = string.Empty; // en|zh|ja|ko
        [MaxLength(20)] public string ExamType { get; set; } = string.Empty; // TOEIC|IELTS|HSK|JLPT|TOPIK
        [MaxLength(20)] public string? Level { get; set; }
        [MaxLength(300)] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; } = 60;
        public int TotalQuestions { get; set; } = 0;
        public int PassScore { get; set; } = 0;
        [MaxLength(500)] public string? AudioUrl { get; set; }
        [MaxLength(500)] public string? ThumbnailUrl { get; set; }
        public int ViewCount { get; set; } = 0;
        public int AttemptCount { get; set; } = 0;
        public int Status { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ExamPartEntities>? Parts { get; set; }
        public ICollection<UserExamAttemptEntities>? Attempts { get; set; }
    }

    [Table("ExamParts", Schema = "sche_dev_HDNXUdemy")]
    public class ExamPartEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdExam { get; set; }
        public int PartNumber { get; set; }
        [MaxLength(200)] public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [MaxLength(500)] public string? AudioUrl { get; set; }
        [MaxLength(500)] public string? ImageUrl { get; set; }
        public int OrderIndex { get; set; } = 0;

        [ForeignKey("IdExam")] public LanguageExamEntities? Exam { get; set; }
        public ICollection<ExamQuestionEntities>? Questions { get; set; }
    }

    [Table("ExamQuestions", Schema = "sche_dev_HDNXUdemy")]
    public class ExamQuestionEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdPart { get; set; }
        public int OrderNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        [MaxLength(500)] public string? QuestionImage { get; set; }
        [MaxLength(500)] public string? AudioUrl { get; set; }
        public string? Explanation { get; set; }
        public int QuestionType { get; set; } = 1; // 1=MCQ, 2=FillBlank

        [ForeignKey("IdPart")] public ExamPartEntities? Part { get; set; }
        public ICollection<ExamAnswerOptionEntities>? AnswerOptions { get; set; }
    }

    [Table("ExamAnswerOptions", Schema = "sche_dev_HDNXUdemy")]
    public class ExamAnswerOptionEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdQuestion { get; set; }
        [MaxLength(5)] public string Label { get; set; } = string.Empty; // A, B, C, D
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;

        [ForeignKey("IdQuestion")] public ExamQuestionEntities? Question { get; set; }
    }

    [Table("UserExamAttempts", Schema = "sche_dev_HDNXUdemy")]
    public class UserExamAttemptEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdUser { get; set; }
        public int IdExam { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? SubmitTime { get; set; }
        public int TotalScore { get; set; } = 0;
        public int MaxScore { get; set; } = 0;
        public decimal Percentage { get; set; } = 0;
        public int CorrectCount { get; set; } = 0;
        public int Status { get; set; } = 0; // 0=InProgress, 1=Submitted, 2=Expired
        public string? AnswersJson { get; set; }
        public int TimeTakenSeconds { get; set; } = 0;

        [ForeignKey("IdExam")] public LanguageExamEntities? Exam { get; set; }
    }

    [Table("VocabularyItems", Schema = "sche_dev_HDNXUdemy")]
    public class VocabularyItemEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(5)] public string Language { get; set; } = string.Empty;
        [MaxLength(200)] public string Word { get; set; } = string.Empty;
        [MaxLength(200)] public string? Pronunciation { get; set; }
        public string MeaningVi { get; set; } = string.Empty;
        public string? MeaningEn { get; set; }
        public string? ExampleSentence { get; set; }
        public string? ExampleTranslation { get; set; }
        [MaxLength(500)] public string? AudioUrl { get; set; }
        [MaxLength(500)] public string? ImageUrl { get; set; }
        [MaxLength(20)] public string? Level { get; set; }
        [MaxLength(500)] public string? Tags { get; set; }
        public int Status { get; set; } = 1;
    }

    [Table("UserVocabProgress", Schema = "sche_dev_HDNXUdemy")]
    public class UserVocabProgressEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdUser { get; set; }
        public int IdVocab { get; set; }
        public int Repetition { get; set; } = 0;
        public decimal EaseFactor { get; set; } = 2.5m;
        public int Interval { get; set; } = 1;
        public DateTime NextReview { get; set; } = DateTime.UtcNow;
        public bool IsMastered { get; set; } = false;
        public DateTime LastReviewed { get; set; } = DateTime.UtcNow;

        [ForeignKey("IdVocab")] public VocabularyItemEntities? Vocab { get; set; }
    }
}
