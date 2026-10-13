using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HDNXUdemyServices.Services
{
    public class LanguageExamService : ILanguageExamService
    {
        private readonly ProjectContext _context;

        public LanguageExamService(ProjectContext context) => _context = context;

        // ── List exams ───────────────────────────────────────────────────────
        public async Task<List<LanguageExamModel>> GetExamListAsync(
            string language, string? examType, string? level, int page, int pageSize)
        {
            var query = _context.LanguageExams!
                .Where(e => e.Language == language && e.Status == 1);

            if (!string.IsNullOrEmpty(examType))
                query = query.Where(e => e.ExamType == examType);
            if (!string.IsNullOrEmpty(level))
                query = query.Where(e => e.Level == level);

            return await query
                .OrderByDescending(e => e.AttemptCount)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(e => new LanguageExamModel
                {
                    Id = e.Id, Language = e.Language, ExamType = e.ExamType,
                    Level = e.Level, Title = e.Title, Description = e.Description,
                    DurationMinutes = e.DurationMinutes, TotalQuestions = e.TotalQuestions,
                    PassScore = e.PassScore, ThumbnailUrl = e.ThumbnailUrl,
                    ViewCount = e.ViewCount, AttemptCount = e.AttemptCount
                }).ToListAsync();
        }

        // ── Exam detail (no correct answers exposed) ─────────────────────────
        public async Task<LanguageExamDetailModel?> GetExamDetailAsync(int examId)
        {
            var exam = await _context.LanguageExams!
                .Include(e => e.Parts!.OrderBy(p => p.OrderIndex))
                    .ThenInclude(p => p.Questions!.OrderBy(q => q.OrderNumber))
                        .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(e => e.Id == examId && e.Status == 1);

            if (exam == null) return null;

            // Increment view count
            exam.ViewCount++;
            await _context.SaveChangesAsync();

            return MapToDetail(exam);
        }

        // ── Start exam → create attempt ──────────────────────────────────────
        public async Task<StartExamResponse> StartExamAsync(int examId, int userId)
        {
            var exam = await _context.LanguageExams!
                .Include(e => e.Parts!.OrderBy(p => p.OrderIndex))
                    .ThenInclude(p => p.Questions!.OrderBy(q => q.OrderNumber))
                        .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(e => e.Id == examId && e.Status == 1)
                ?? throw new Exception("Đề thi không tồn tại.");

            var now = DateTime.UtcNow;
            var attempt = new HDNXUdemyData.Entities.UserExamAttemptEntities
            {
                IdUser = userId,
                IdExam = examId,
                StartTime = now,
                Status = 0,
                MaxScore = exam.TotalQuestions
            };
            _context.UserExamAttempts!.Add(attempt);

            exam.AttemptCount++;
            await _context.SaveChangesAsync();

            return new StartExamResponse
            {
                AttemptId = attempt.Id,
                ExamId = examId,
                StartTime = now,
                Deadline = now.AddMinutes(exam.DurationMinutes),
                DurationSeconds = exam.DurationMinutes * 60,
                Exam = MapToDetail(exam)
            };
        }

        // ── Submit exam ──────────────────────────────────────────────────────
        public async Task<ExamResultModel> SubmitExamAsync(
            int attemptId, int userId, List<UserAnswerRequest> answers)
        {
            var attempt = await _context.UserExamAttempts!
                .Include(a => a.Exam)
                .FirstOrDefaultAsync(a => a.Id == attemptId && a.IdUser == userId)
                ?? throw new Exception("Không tìm thấy bài làm.");

            if (attempt.Status != 0)
                throw new Exception("Bài đã được nộp.");

            // Load all correct answers for this exam
            var questionIds = answers.Select(a => a.QuestionId).ToList();
            var correctAnswers = await _context.ExamAnswerOptions!
                .Where(ao => questionIds.Contains(ao.IdQuestion) && ao.IsCorrect)
                .ToListAsync();

            var allOptions = await _context.ExamAnswerOptions!
                .Where(ao => questionIds.Contains(ao.IdQuestion))
                .ToListAsync();

            var allQuestions = await _context.ExamQuestions!
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            // Grade answers
            var resultItems = new List<object>();
            int correctCount = 0;

            var questionResultModels = new List<QuestionResultModel>();
            foreach (var ans in answers)
            {
                var correctOpt = correctAnswers.FirstOrDefault(c => c.IdQuestion == ans.QuestionId);
                var isCorrect = correctOpt?.Id == ans.SelectedAnswerId;
                if (isCorrect) correctCount++;

                var q = allQuestions.FirstOrDefault(q => q.Id == ans.QuestionId);
                questionResultModels.Add(new QuestionResultModel
                {
                    QuestionId = ans.QuestionId,
                    QuestionText = q?.QuestionText ?? "",
                    SelectedAnswerId = ans.SelectedAnswerId,
                    CorrectAnswerId = correctOpt?.Id ?? 0,
                    IsCorrect = isCorrect,
                    Explanation = q?.Explanation,
                    Options = allOptions
                        .Where(o => o.IdQuestion == ans.QuestionId)
                        .Select(o => new AnswerOptionWithCorrect
                        {
                            Id = o.Id, Label = o.Label,
                            AnswerText = o.AnswerText, IsCorrect = o.IsCorrect
                        }).ToList()
                });
            }

            var now = DateTime.UtcNow;
            var timeTaken = (int)(now - attempt.StartTime).TotalSeconds;
            var percentage = attempt.MaxScore > 0
                ? Math.Round((decimal)correctCount / attempt.MaxScore * 100, 2) : 0;

            attempt.SubmitTime = now;
            attempt.CorrectCount = correctCount;
            attempt.TotalScore = correctCount;
            attempt.Percentage = percentage;
            attempt.TimeTakenSeconds = timeTaken;
            attempt.Status = 1;
            attempt.AnswersJson = JsonSerializer.Serialize(answers);

            await _context.SaveChangesAsync();

            return new ExamResultModel
            {
                AttemptId = attemptId,
                ExamId = attempt.IdExam,
                ExamTitle = attempt.Exam?.Title ?? "",
                TotalScore = correctCount,
                MaxScore = attempt.MaxScore,
                Percentage = percentage,
                CorrectCount = correctCount,
                TotalQuestions = attempt.MaxScore,
                IsPassed = attempt.Exam != null && percentage >= attempt.Exam.PassScore,
                TimeTakenSeconds = timeTaken,
                QuestionResults = questionResultModels
            };
        }

        // ── Get result by attemptId ──────────────────────────────────────────
        public async Task<ExamResultModel?> GetAttemptResultAsync(int attemptId, int userId)
        {
            var attempt = await _context.UserExamAttempts!
                .Include(a => a.Exam)
                .FirstOrDefaultAsync(a => a.Id == attemptId && a.IdUser == userId);

            if (attempt == null || attempt.Status == 0) return null;

            // Re-build results from stored answers
            List<UserAnswerRequest> storedAnswers = new();
            if (!string.IsNullOrEmpty(attempt.AnswersJson))
                storedAnswers = JsonSerializer.Deserialize<List<UserAnswerRequest>>(attempt.AnswersJson) ?? new();

            var questionIds = storedAnswers.Select(a => a.QuestionId).ToList();
            var allOptions = await _context.ExamAnswerOptions!
                .Where(ao => questionIds.Contains(ao.IdQuestion)).ToListAsync();
            var allQuestions = await _context.ExamQuestions!
                .Where(q => questionIds.Contains(q.Id)).ToListAsync();

            var questionResults = storedAnswers.Select(ans =>
            {
                var correctOpt = allOptions.FirstOrDefault(o => o.IdQuestion == ans.QuestionId && o.IsCorrect);
                var q = allQuestions.FirstOrDefault(q => q.Id == ans.QuestionId);
                return new QuestionResultModel
                {
                    QuestionId = ans.QuestionId,
                    QuestionText = q?.QuestionText ?? "",
                    SelectedAnswerId = ans.SelectedAnswerId,
                    CorrectAnswerId = correctOpt?.Id ?? 0,
                    IsCorrect = correctOpt?.Id == ans.SelectedAnswerId,
                    Explanation = q?.Explanation,
                    Options = allOptions.Where(o => o.IdQuestion == ans.QuestionId)
                        .Select(o => new AnswerOptionWithCorrect
                        { Id = o.Id, Label = o.Label, AnswerText = o.AnswerText, IsCorrect = o.IsCorrect })
                        .ToList()
                };
            }).ToList();

            return new ExamResultModel
            {
                AttemptId = attempt.Id,
                ExamId = attempt.IdExam,
                ExamTitle = attempt.Exam?.Title ?? "",
                TotalScore = attempt.TotalScore,
                MaxScore = attempt.MaxScore,
                Percentage = attempt.Percentage,
                CorrectCount = attempt.CorrectCount,
                TotalQuestions = attempt.MaxScore,
                IsPassed = attempt.Exam != null && attempt.CorrectCount >= attempt.Exam.PassScore,
                TimeTakenSeconds = attempt.TimeTakenSeconds,
                QuestionResults = questionResults
            };
        }

        // ── Leaderboard ──────────────────────────────────────────────────────
        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync(int examId, int top = 10)
        {
            return await _context.UserExamAttempts!
                .Where(a => a.IdExam == examId && a.Status == 1)
                .OrderByDescending(a => a.Percentage)
                .ThenBy(a => a.TimeTakenSeconds)
                .Take(top)
                .Select((a, index) => new LeaderboardEntry
                {
                    Rank = 0, // set after fetch
                    UserName = "Học viên #" + a.IdUser,
                    Percentage = a.Percentage,
                    TimeTakenSeconds = a.TimeTakenSeconds,
                    SubmitTime = a.SubmitTime ?? DateTime.UtcNow
                }).ToListAsync()
                .ContinueWith(t =>
                {
                    var list = t.Result;
                    for (int i = 0; i < list.Count; i++) list[i].Rank = i + 1;
                    return list;
                });
        }

        // ── Helper ───────────────────────────────────────────────────────────
        private static LanguageExamDetailModel MapToDetail(HDNXUdemyData.Entities.LanguageExamEntities e) => new()
        {
            Id = e.Id, Language = e.Language, ExamType = e.ExamType, Level = e.Level,
            Title = e.Title, Description = e.Description, DurationMinutes = e.DurationMinutes,
            TotalQuestions = e.TotalQuestions, PassScore = e.PassScore,
            ThumbnailUrl = e.ThumbnailUrl, ViewCount = e.ViewCount, AttemptCount = e.AttemptCount,
            Parts = e.Parts?.Select(p => new ExamPartModel
            {
                Id = p.Id, PartNumber = p.PartNumber, Title = p.Title,
                Description = p.Description, AudioUrl = p.AudioUrl, ImageUrl = p.ImageUrl,
                Questions = p.Questions?.Select(q => new ExamQuestionModel
                {
                    Id = q.Id, OrderNumber = q.OrderNumber, QuestionText = q.QuestionText,
                    QuestionImage = q.QuestionImage, AudioUrl = q.AudioUrl, QuestionType = q.QuestionType,
                    Options = q.AnswerOptions?.Select(o => new AnswerOptionModel
                    { Id = o.Id, Label = o.Label, AnswerText = o.AnswerText }).ToList() ?? new()
                }).ToList() ?? new()
            }).ToList() ?? new()
        };

        // ── ADMIN CRUD ───────────────────────────────────────────────────────
        public async Task<LanguageExamModel> CreateExamAsync(CreateExamRequest req)
        {
            var exam = new HDNXUdemyData.Entities.LanguageExamEntities
            {
                Language = req.Language, ExamType = req.ExamType, Level = req.Level,
                Title = req.Title, Description = req.Description,
                DurationMinutes = req.DurationMinutes, TotalQuestions = req.TotalQuestions,
                PassScore = req.PassScore, ThumbnailUrl = req.ThumbnailUrl,
                Status = 1, CreatedAt = DateTime.UtcNow
            };
            _context.LanguageExams!.Add(exam);
            await _context.SaveChangesAsync();
            return new LanguageExamModel { Id = exam.Id, Language = exam.Language, ExamType = exam.ExamType,
                Level = exam.Level, Title = exam.Title, DurationMinutes = exam.DurationMinutes,
                TotalQuestions = exam.TotalQuestions, PassScore = exam.PassScore };
        }

        public async Task<bool> UpdateExamAsync(int examId, CreateExamRequest req)
        {
            var exam = await _context.LanguageExams!.FindAsync(examId);
            if (exam == null) return false;
            exam.Language = req.Language; exam.ExamType = req.ExamType; exam.Level = req.Level;
            exam.Title = req.Title; exam.Description = req.Description;
            exam.DurationMinutes = req.DurationMinutes; exam.TotalQuestions = req.TotalQuestions;
            exam.PassScore = req.PassScore; exam.ThumbnailUrl = req.ThumbnailUrl;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExamAsync(int examId)
        {
            var exam = await _context.LanguageExams!.FindAsync(examId);
            if (exam == null) return false;
            exam.Status = 0;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ExamQuestionAdminModel>> GetQuestionsWithAnswersAsync(int examId)
        {
            var parts = await _context.ExamParts!
                .Include(p => p.Questions!).ThenInclude(q => q.AnswerOptions)
                .Where(p => p.IdExam == examId).ToListAsync();

            return parts.SelectMany(p => p.Questions?.ToList() ?? new List<HDNXUdemyData.Entities.ExamQuestionEntities>())
                .Select(q => new ExamQuestionAdminModel
                {
                    Id = q.Id, OrderNumber = q.OrderNumber, QuestionText = q.QuestionText,
                    QuestionImage = q.QuestionImage, AudioUrl = q.AudioUrl, QuestionType = q.QuestionType,
                    AnswerOptions = q.AnswerOptions?.Select(o => new AnswerOptionWithCorrect
                    { Id = o.Id, Label = o.Label, AnswerText = o.AnswerText, IsCorrect = o.IsCorrect })
                    .ToList() ?? new List<AnswerOptionWithCorrect>()
                }).ToList();
        }

        public async Task<int> CreateQuestionAsync(CreateQuestionRequest req)
        {
            var q = new HDNXUdemyData.Entities.ExamQuestionEntities
            {
                IdPart = req.IdPart, OrderNumber = req.OrderNumber, QuestionText = req.QuestionText,
                QuestionImage = req.QuestionImage, AudioUrl = req.AudioUrl,
                Explanation = req.Explanation, QuestionType = req.QuestionType
            };
            _context.ExamQuestions!.Add(q);
            await _context.SaveChangesAsync();
            foreach (var ans in req.Answers)
                _context.ExamAnswerOptions!.Add(new HDNXUdemyData.Entities.ExamAnswerOptionEntities
                { IdQuestion = q.Id, Label = ans.Label, AnswerText = ans.AnswerText, IsCorrect = ans.IsCorrect });
            await _context.SaveChangesAsync();
            return q.Id;
        }

        public async Task<bool> UpdateQuestionAsync(int questionId, CreateQuestionRequest req)
        {
            var q = await _context.ExamQuestions!.FindAsync(questionId);
            if (q == null) return false;
            q.QuestionText = req.QuestionText; q.Explanation = req.Explanation;
            q.OrderNumber = req.OrderNumber; q.QuestionImage = req.QuestionImage;
            // Replace answer options
            var oldOpts = _context.ExamAnswerOptions!.Where(o => o.IdQuestion == questionId);
            _context.ExamAnswerOptions!.RemoveRange(oldOpts);
            foreach (var ans in req.Answers)
                _context.ExamAnswerOptions!.Add(new HDNXUdemyData.Entities.ExamAnswerOptionEntities
                { IdQuestion = questionId, Label = ans.Label, AnswerText = ans.AnswerText, IsCorrect = ans.IsCorrect });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var q = await _context.ExamQuestions!.FindAsync(questionId);
            if (q == null) return false;
            _context.ExamQuestions!.Remove(q);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── User History ─────────────────────────────────────────────────────
        public async Task<List<ExamHistoryModel>> GetUserHistoryAsync(int userId, string? language, int page, int pageSize)
        {
            var query = _context.UserExamAttempts!
                .Include(a => a.Exam)
                .Where(a => a.IdUser == userId && a.Status == 1);
            if (!string.IsNullOrEmpty(language))
                query = query.Where(a => a.Exam != null && a.Exam.Language == language);
            var items = await query
                .OrderByDescending(a => a.SubmitTime)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();
            return items.Select(a => new ExamHistoryModel
            {
                AttemptId = a.Id, ExamId = a.IdExam,
                ExamTitle  = a.Exam?.Title ?? "",
                Language   = a.Exam?.Language ?? "",
                ExamType   = a.Exam?.ExamType ?? "",
                Level      = a.Exam?.Level,
                Percentage     = a.Percentage,
                CorrectCount   = a.CorrectCount,
                TotalQuestions = a.MaxScore,
                IsPassed = a.Exam != null && a.Percentage >= a.Exam.PassScore,
                TimeTakenSeconds = a.TimeTakenSeconds,
                SubmitTime = a.SubmitTime ?? DateTime.UtcNow
            }).ToList();
        }
    }

    // ── VocabularyService (SM-2 Spaced Repetition) ───────────────────────────
    public class VocabularyService : IVocabularyService
    {
        private readonly ProjectContext _context;
        public VocabularyService(ProjectContext context) => _context = context;

        public async Task<List<VocabularyModel>> GetVocabularyListAsync(
            string language, string? level, string? tags, int page, int pageSize)
        {
            var query = _context.VocabularyItems!
                .Where(v => v.Language == language && v.Status == 1);
            if (!string.IsNullOrEmpty(level)) query = query.Where(v => v.Level == level);
            if (!string.IsNullOrEmpty(tags)) query = query.Where(v => v.Tags != null && v.Tags.Contains(tags));

            return await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(v => MapVocab(v, null)).ToListAsync();
        }

        public async Task<List<VocabularyModel>> GetReviewItemsAsync(int userId, string language, int limit = 20)
        {
            var now = DateTime.UtcNow;
            return await (
                from v in _context.VocabularyItems!
                join p in _context.UserVocabProgress! on v.Id equals p.IdVocab into progress
                from p in progress.Where(x => x.IdUser == userId).DefaultIfEmpty()
                where v.Language == language && v.Status == 1
                    && (p == null || p.NextReview <= now)
                orderby p == null ? 0 : 1, p != null ? p.NextReview : now
                select new VocabularyModel
                {
                    Id = v.Id, Language = v.Language, Word = v.Word,
                    Pronunciation = v.Pronunciation, MeaningVi = v.MeaningVi,
                    MeaningEn = v.MeaningEn, ExampleSentence = v.ExampleSentence,
                    ExampleTranslation = v.ExampleTranslation, AudioUrl = v.AudioUrl,
                    ImageUrl = v.ImageUrl, Level = v.Level, Tags = v.Tags,
                    NextReview = p != null ? p.NextReview : now,
                    IsMastered = p != null && p.IsMastered,
                    Repetition = p != null ? p.Repetition : 0
                }
            ).Take(limit).ToListAsync();
        }

        /// <summary>SM-2 Spaced Repetition Algorithm. quality: 0=fail, 3=hard, 4=good, 5=perfect</summary>
        public async Task UpdateVocabProgressAsync(int userId, int vocabId, int quality)
        {
            var progress = await _context.UserVocabProgress!
                .FirstOrDefaultAsync(p => p.IdUser == userId && p.IdVocab == vocabId);

            if (progress == null)
            {
                progress = new HDNXUdemyData.Entities.UserVocabProgressEntities
                { IdUser = userId, IdVocab = vocabId };
                _context.UserVocabProgress!.Add(progress);
            }

            // SM-2 algorithm
            if (quality >= 3)
            {
                progress.Interval = progress.Repetition switch
                {
                    0 => 1,
                    1 => 6,
                    _ => (int)Math.Round(progress.Interval * progress.EaseFactor)
                };
                progress.Repetition++;
            }
            else
            {
                progress.Repetition = 0;
                progress.Interval = 1;
            }

            progress.EaseFactor = Math.Max(1.3m,
                progress.EaseFactor + 0.1m - (5 - quality) * (0.08m + (5 - quality) * 0.02m));
            progress.NextReview = DateTime.UtcNow.AddDays(progress.Interval);
            progress.LastReviewed = DateTime.UtcNow;
            progress.IsMastered = progress.Repetition >= 5 && quality >= 4;

            await _context.SaveChangesAsync();
        }

        public async Task<VocabStatsModel> GetVocabStatsAsync(int userId, string language)
        {
            var total = await _context.VocabularyItems!.CountAsync(v => v.Language == language && v.Status == 1);
            var progress = await _context.UserVocabProgress!
                .Where(p => p.IdUser == userId)
                .Join(_context.VocabularyItems!, p => p.IdVocab, v => v.Id, (p, v) => new { p, v })
                .Where(x => x.v.Language == language)
                .ToListAsync();

            var mastered = progress.Count(x => x.p.IsMastered);
            var dueNow = progress.Count(x => x.p.NextReview <= DateTime.UtcNow && !x.p.IsMastered);

            return new VocabStatsModel
            {
                Language = language,
                TotalLearned = progress.Count,
                MasteredCount = mastered,
                DueForReview = dueNow,
                NewToday = 0,
                ProgressPercent = total > 0 ? Math.Round((double)mastered / total * 100, 1) : 0
            };
        }

        private static VocabularyModel MapVocab(
            HDNXUdemyData.Entities.VocabularyItemEntities v,
            HDNXUdemyData.Entities.UserVocabProgressEntities? p) => new()
        {
            Id = v.Id, Language = v.Language, Word = v.Word,
            Pronunciation = v.Pronunciation, MeaningVi = v.MeaningVi, MeaningEn = v.MeaningEn,
            ExampleSentence = v.ExampleSentence, ExampleTranslation = v.ExampleTranslation,
            AudioUrl = v.AudioUrl, ImageUrl = v.ImageUrl, Level = v.Level, Tags = v.Tags,
            IsMastered = p?.IsMastered ?? false, Repetition = p?.Repetition ?? 0
        };

        // ── ADMIN CRUD ───────────────────────────────────────────────────────
        public async Task<int> CreateVocabAsync(CreateVocabRequest req)
        {
            var v = new HDNXUdemyData.Entities.VocabularyItemEntities
            {
                Language = req.Language, Word = req.Word, Pronunciation = req.Pronunciation,
                MeaningVi = req.MeaningVi, MeaningEn = req.MeaningEn,
                ExampleSentence = req.ExampleSentence, ExampleTranslation = req.ExampleTranslation,
                AudioUrl = req.AudioUrl, ImageUrl = req.ImageUrl, Level = req.Level, Tags = req.Tags,
                Status = 1
            };
            _context.VocabularyItems!.Add(v);
            await _context.SaveChangesAsync();
            return v.Id;
        }

        public async Task<bool> UpdateVocabAsync(int vocabId, CreateVocabRequest req)
        {
            var v = await _context.VocabularyItems!.FindAsync(vocabId);
            if (v == null) return false;
            v.Word = req.Word; v.Pronunciation = req.Pronunciation; v.MeaningVi = req.MeaningVi;
            v.MeaningEn = req.MeaningEn; v.ExampleSentence = req.ExampleSentence;
            v.ExampleTranslation = req.ExampleTranslation; v.Level = req.Level; v.Tags = req.Tags;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVocabAsync(int vocabId)
        {
            var v = await _context.VocabularyItems!.FindAsync(vocabId);
            if (v == null) return false;
            v.Status = 0;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> BulkImportVocabAsync(string language, List<CreateVocabRequest> items)
        {
            var entities = items.Select(req => new HDNXUdemyData.Entities.VocabularyItemEntities
            {
                Language = language, Word = req.Word, Pronunciation = req.Pronunciation,
                MeaningVi = req.MeaningVi, MeaningEn = req.MeaningEn,
                ExampleSentence = req.ExampleSentence, ExampleTranslation = req.ExampleTranslation,
                AudioUrl = req.AudioUrl, ImageUrl = req.ImageUrl, Level = req.Level, Tags = req.Tags,
                Status = 1
            }).ToList();
            _context.VocabularyItems!.AddRange(entities);
            await _context.SaveChangesAsync();
            return (int)entities.Count;
        }

        // ── Streak ───────────────────────────────────────────────────────────
        public async Task<StreakModel> GetStreakAsync(int userId, string language)
        {
            var streak = await _context.UserLearningStreaks!
                .FirstOrDefaultAsync(s => s.IdUser == userId && s.Language == language);
            if (streak == null) return new StreakModel();
            return new StreakModel
            {
                CurrentStreak = streak.CurrentStreak,
                LongestStreak = streak.LongestStreak,
                TotalActiveDays = streak.TotalActiveDays,
                IsActiveToday = streak.LastActiveDate.Date == DateTime.UtcNow.Date
            };
        }

        public async Task UpdateStreakAsync(int userId, string language)
        {
            var today = DateTime.UtcNow.Date;
            var streak = await _context.UserLearningStreaks!
                .FirstOrDefaultAsync(s => s.IdUser == userId && s.Language == language);
            if (streak == null)
            {
                _context.UserLearningStreaks!.Add(new HDNXUdemyData.Entities.UserLearningStreakEntities
                { IdUser = userId, Language = language, LastActiveDate = today, CurrentStreak = 1, LongestStreak = 1, TotalActiveDays = 1 });
            }
            else if (streak.LastActiveDate.Date == today) { return; }
            else
            {
                var daysDiff = (today - streak.LastActiveDate.Date).Days;
                streak.CurrentStreak = daysDiff == 1 ? streak.CurrentStreak + 1 : 1;
                streak.LongestStreak = Math.Max(streak.LongestStreak, streak.CurrentStreak);
                streak.TotalActiveDays++;
                streak.LastActiveDate = today;
            }
            await _context.SaveChangesAsync();
        }
    }
}
