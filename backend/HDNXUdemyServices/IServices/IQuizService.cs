using HDNXUdemyModel.Model;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;

namespace HDNXUdemyServices.IServices
{
    public interface IQuizService
    {
        Task<QuizModel> CreateQuiz(QuizModel model);
        Task<List<QuizModel>> GetQuizzesByCourse(long idCourse);
        Task<QuizModel?> GetQuiz(long id);
        Task<bool> UpdateQuiz(long id, QuizModel model);
        Task<QuizAttemptResultModel> SubmitQuiz(SubmitQuizRequest request, long idStudent);
    }
}
