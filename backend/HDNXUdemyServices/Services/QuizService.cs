using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace HDNXUdemyServices.Services
{
    public class QuizService : IQuizService
    {
        private readonly ProjectContext _context;
        private readonly IAuditLogService _auditLogService;

        public QuizService(IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService)
        {
            _context = new ProjectContext(httpContextAccessor);
            _auditLogService = auditLogService;
        }

        public async Task<QuizModel> CreateQuiz(QuizModel model)
        {
            ValidateQuiz(model);
            var quiz = new QuizEntities
            {
                IdCourse = model.IdCourse,
                Title = model.Title,
                Description = model.Description,
                PassingScore = model.PassingScore,
                Status = (int)EStatus.Active
            };
            _context.Quizzes!.Add(quiz);
            await _context.SaveChangesAsync();

            foreach (var questionModel in model.Questions)
            {
                var question = new QuizQuestionEntities
                {
                    IdQuiz = quiz.Id,
                    QuestionText = questionModel.QuestionText,
                    Point = questionModel.Point <= 0 ? 1 : questionModel.Point,
                    Status = (int)EStatus.Active
                };
                _context.QuizQuestions!.Add(question);
                await _context.SaveChangesAsync();

                _context.QuizAnswerOptions!.AddRange(questionModel.Answers.Select(answer => new QuizAnswerOptionEntities
                {
                    IdQuestion = question.Id,
                    AnswerText = answer.AnswerText,
                    IsCorrect = answer.IsCorrect,
                    Status = (int)EStatus.Active
                }));
                await _context.SaveChangesAsync();
            }

            await _auditLogService.WriteAsync(null, "Quiz.Create", "Quiz", quiz.Id.ToString(), null, model);
            return (await GetQuiz(quiz.Id))!;
        }

        public async Task<List<QuizModel>> GetQuizzesByCourse(long idCourse)
        {
            var quizzes = await _context.Quizzes!.AsNoTracking().Where(x => x.IdCourse == idCourse).ToListAsync();
            var result = new List<QuizModel>();
            foreach (var quiz in quizzes)
            {
                result.Add((await GetQuiz(quiz.Id))!);
            }
            return result;
        }

        public async Task<QuizModel?> GetQuiz(long id)
        {
            var quiz = await _context.Quizzes!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (quiz == null) return null;
            var questions = await _context.QuizQuestions!.AsNoTracking().Where(x => x.IdQuiz == id).ToListAsync();
            var questionIds = questions.Select(x => x.Id).ToList();
            var answers = await _context.QuizAnswerOptions!.AsNoTracking().Where(x => questionIds.Contains(x.IdQuestion)).ToListAsync();
            return new QuizModel
            {
                Id = quiz.Id,
                IdCourse = quiz.IdCourse,
                Title = quiz.Title,
                Description = quiz.Description,
                PassingScore = quiz.PassingScore,
                Status = quiz.Status ?? (int)EStatus.Active,
                Questions = questions.Select(q => new QuizQuestionModel
                {
                    Id = q.Id,
                    IdQuiz = q.IdQuiz,
                    QuestionText = q.QuestionText,
                    Point = q.Point,
                    Status = q.Status ?? (int)EStatus.Active,
                    Answers = answers.Where(a => a.IdQuestion == q.Id).Select(a => new QuizAnswerOptionModel
                    {
                        Id = a.Id,
                        IdQuestion = a.IdQuestion,
                        AnswerText = a.AnswerText,
                        IsCorrect = a.IsCorrect,
                        Status = a.Status ?? (int)EStatus.Active
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<bool> UpdateQuiz(long id, QuizModel model)
        {
            ValidateQuiz(model);
            var quiz = await _context.Quizzes!.FindAsync(id) ?? throw new ProjectNotFoundException("Quiz not found.");
            var oldValue = await GetQuiz(id);
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.PassingScore = model.PassingScore;
            quiz.Status = model.Status;

            var existingQuestions = await _context.QuizQuestions!.Where(x => x.IdQuiz == id).ToListAsync();
            var existingQuestionIds = existingQuestions.Select(x => x.Id).ToList();
            var existingAnswers = await _context.QuizAnswerOptions!.Where(x => existingQuestionIds.Contains(x.IdQuestion)).ToListAsync();
            _context.QuizAnswerOptions!.RemoveRange(existingAnswers);
            _context.QuizQuestions!.RemoveRange(existingQuestions);
            await _context.SaveChangesAsync();

            foreach (var questionModel in model.Questions)
            {
                var question = new QuizQuestionEntities
                {
                    IdQuiz = quiz.Id,
                    QuestionText = questionModel.QuestionText,
                    Point = questionModel.Point <= 0 ? 1 : questionModel.Point,
                    Status = (int)EStatus.Active
                };
                _context.QuizQuestions!.Add(question);
                await _context.SaveChangesAsync();

                _context.QuizAnswerOptions!.AddRange(questionModel.Answers.Select(answer => new QuizAnswerOptionEntities
                {
                    IdQuestion = question.Id,
                    AnswerText = answer.AnswerText,
                    IsCorrect = answer.IsCorrect,
                    Status = (int)EStatus.Active
                }));
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            await _auditLogService.WriteAsync(null, "Quiz.Update", "Quiz", id.ToString(), oldValue, model);
            return true;
        }

        public async Task<QuizAttemptResultModel> SubmitQuiz(SubmitQuizRequest request, long idStudent)
        {
            if (idStudent <= 0) throw new ProjectBadRequestException("Student is required.");
            var quiz = await _context.Quizzes!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.IdQuiz)
                ?? throw new ProjectNotFoundException("Quiz not found.");
            var questions = await _context.QuizQuestions!.AsNoTracking().Where(x => x.IdQuiz == request.IdQuiz).ToListAsync();
            var questionIds = questions.Select(x => x.Id).ToList();
            var correctAnswers = await _context.QuizAnswerOptions!.AsNoTracking()
                .Where(x => questionIds.Contains(x.IdQuestion) && x.IsCorrect)
                .ToListAsync();
            if (request.Answers == null || request.Answers.Select(x => x.IdQuestion).Distinct().Count() < questions.Count)
            {
                throw new ProjectBadRequestException("Every question must be answered.");
            }

            int score = 0;
            var answerRows = new List<QuizAttemptAnswerEntities>();
            foreach (var answer in request.Answers)
            {
                var question = questions.FirstOrDefault(x => x.Id == answer.IdQuestion);
                var isCorrect = correctAnswers.Any(x => x.Id == answer.IdAnswerOption && x.IdQuestion == answer.IdQuestion);
                if (question != null && isCorrect)
                {
                    score += question.Point;
                }
                answerRows.Add(new QuizAttemptAnswerEntities
                {
                    IdQuestion = answer.IdQuestion,
                    IdAnswerOption = answer.IdAnswerOption,
                    IsCorrect = isCorrect,
                    Status = (int)EStatus.Active
                });
            }

            var totalScore = questions.Sum(x => x.Point);
            var scorePercent = totalScore == 0 ? 0 : (int)Math.Round(score * 100m / totalScore);
            var attempt = new QuizAttemptEntities
            {
                IdQuiz = quiz.Id,
                IdStudent = idStudent,
                Score = scorePercent,
                IsPassed = scorePercent >= quiz.PassingScore,
                SubmittedAt = LocalDateTime.FromDateTime(DateTime.UtcNow),
                Status = (int)EStatus.Active
            };
            _context.QuizAttempts!.Add(attempt);
            await _context.SaveChangesAsync();
            foreach (var row in answerRows)
            {
                row.IdAttempt = attempt.Id;
            }
            _context.QuizAttemptAnswers!.AddRange(answerRows);
            await _context.SaveChangesAsync();
            await _auditLogService.WriteAsync(idStudent, "Quiz.Submit", "QuizAttempt", attempt.Id.ToString(), null, new { request.IdQuiz, scorePercent });

            return new QuizAttemptResultModel
            {
                AttemptId = attempt.Id,
                Score = scorePercent,
                TotalScore = 100,
                PassingScore = quiz.PassingScore,
                IsPassed = attempt.IsPassed
            };
        }

        private static void ValidateQuiz(QuizModel model)
        {
            if (model.IdCourse <= 0) throw new ProjectBadRequestException("Course is required.");
            if (string.IsNullOrWhiteSpace(model.Title)) throw new ProjectBadRequestException("Title is required.");
            if (model.PassingScore is < 0 or > 100) throw new ProjectBadRequestException("PassingScore must be between 0 and 100.");
            if (model.Questions == null || model.Questions.Count == 0) throw new ProjectBadRequestException("Quiz needs at least one question.");
            foreach (var question in model.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.QuestionText)) throw new ProjectBadRequestException("QuestionText is required.");
                if (question.Answers.Count < 2) throw new ProjectBadRequestException("Each question needs at least two answers.");
                if (!question.Answers.Any(x => x.IsCorrect)) throw new ProjectBadRequestException("Each question needs a correct answer.");
                if (question.Answers.Any(x => string.IsNullOrWhiteSpace(x.AnswerText))) throw new ProjectBadRequestException("AnswerText is required.");
            }
        }
    }
}
