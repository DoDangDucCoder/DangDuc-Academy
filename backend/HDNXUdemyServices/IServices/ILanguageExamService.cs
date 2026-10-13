using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;

namespace HDNXUdemyServices.IServices
{
    public interface ILanguageExamService
    {
        Task<List<LanguageExamModel>> GetExamListAsync(string language, string? examType, string? level, int page, int pageSize);
        Task<LanguageExamDetailModel?> GetExamDetailAsync(int examId);
        Task<StartExamResponse> StartExamAsync(int examId, int userId);
        Task<ExamResultModel> SubmitExamAsync(int attemptId, int userId, List<UserAnswerRequest> answers);
        Task<ExamResultModel?> GetAttemptResultAsync(int attemptId, int userId);
        Task<List<LeaderboardEntry>> GetLeaderboardAsync(int examId, int top = 10);
        // Admin CRUD
        Task<LanguageExamModel> CreateExamAsync(CreateExamRequest model);
        Task<bool> UpdateExamAsync(int examId, CreateExamRequest model);
        Task<bool> DeleteExamAsync(int examId);
        Task<List<ExamQuestionAdminModel>> GetQuestionsWithAnswersAsync(int examId);
        Task<int> CreateQuestionAsync(CreateQuestionRequest model);
        Task<bool> UpdateQuestionAsync(int questionId, CreateQuestionRequest model);
        Task<bool> DeleteQuestionAsync(int questionId);
    }

    public interface IVocabularyService
    {
        Task<List<VocabularyModel>> GetVocabularyListAsync(string language, string? level, string? tags, int page, int pageSize);
        Task<List<VocabularyModel>> GetReviewItemsAsync(int userId, string language, int limit = 20);
        Task UpdateVocabProgressAsync(int userId, int vocabId, int quality); // quality: 0-5 (SM-2 algo)
        Task<VocabStatsModel> GetVocabStatsAsync(int userId, string language);
        // Admin CRUD
        Task<int> CreateVocabAsync(CreateVocabRequest model);
        Task<bool> UpdateVocabAsync(int vocabId, CreateVocabRequest model);
        Task<bool> DeleteVocabAsync(int vocabId);
        Task<int> BulkImportVocabAsync(string language, List<CreateVocabRequest> items);
    }
}
