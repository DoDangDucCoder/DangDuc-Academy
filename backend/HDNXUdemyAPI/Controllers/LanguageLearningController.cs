using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HDNXUdemyAPI.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/language")]
    public class LanguageLearningController : ControllerBase
    {
        private readonly ILanguageExamService _examService;
        private readonly IVocabularyService _vocabService;
        private readonly ILogServices<LanguageLearningController> _logger;

        public LanguageLearningController(
            ILanguageExamService examService,
            IVocabularyService vocabService,
            ILogServices<LanguageLearningController> logger)
        {
            _examService = examService;
            _vocabService = vocabService;
            _logger = logger;
        }

        // ────────────────────── EXAM ENDPOINTS ──────────────────────────────

        /// <summary>Lấy danh sách đề thi theo ngôn ngữ</summary>
        [HttpGet("exams")]
        public async Task<IActionResult> GetExams(
            [FromQuery] string language,
            [FromQuery] string? examType = null,
            [FromQuery] string? level = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            if (string.IsNullOrEmpty(language))
                return BadRequest(new RepositoryModel<object> { StatusCode = 400, SystemMessage = "language là bắt buộc (en/zh/ja/ko)" });

            var data = await _examService.GetExamListAsync(language, examType, level, page, pageSize);
            return Ok(new RepositoryModel<List<LanguageExamModel>> { Data = data, StatusCode = 200 });
        }

        /// <summary>Chi tiết đề thi (không có đáp án đúng)</summary>
        [HttpGet("exams/{examId:int}")]
        public async Task<IActionResult> GetExamDetail(int examId)
        {
            var data = await _examService.GetExamDetailAsync(examId);
            if (data == null) return NotFound();
            return Ok(new RepositoryModel<LanguageExamDetailModel> { Data = data, StatusCode = 200 });
        }

        /// <summary>Bắt đầu làm bài — tạo attempt mới</summary>
        [HttpPost("exams/{examId:int}/start")]
        [Authorize]
        public async Task<IActionResult> StartExam(int examId)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var result = await _examService.StartExamAsync(examId, userId);
            return Ok(new RepositoryModel<StartExamResponse> { Data = result, StatusCode = 200 });
        }

        /// <summary>Nộp bài</summary>
        [HttpPost("exams/attempts/{attemptId:int}/submit")]
        [Authorize]
        public async Task<IActionResult> SubmitExam(int attemptId, [FromBody] List<UserAnswerRequest> answers)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var result = await _examService.SubmitExamAsync(attemptId, userId, answers);
            return Ok(new RepositoryModel<ExamResultModel> { Data = result, StatusCode = 200 });
        }

        /// <summary>Xem kết quả bài đã nộp</summary>
        [HttpGet("exams/attempts/{attemptId:int}/result")]
        [Authorize]
        public async Task<IActionResult> GetResult(int attemptId)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var result = await _examService.GetAttemptResultAsync(attemptId, userId);
            if (result == null) return NotFound();
            return Ok(new RepositoryModel<ExamResultModel> { Data = result, StatusCode = 200 });
        }

        /// <summary>Bảng xếp hạng đề thi</summary>
        [HttpGet("exams/{examId:int}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(int examId, [FromQuery] int top = 10)
        {
            var data = await _examService.GetLeaderboardAsync(examId, top);
            return Ok(new RepositoryModel<List<LeaderboardEntry>> { Data = data, StatusCode = 200 });
        }

        // ────────────────────── VOCABULARY ENDPOINTS ────────────────────────

        /// <summary>Danh sách từ vựng</summary>
        [HttpGet("vocabulary")]
        public async Task<IActionResult> GetVocabulary(
            [FromQuery] string language,
            [FromQuery] string? level = null,
            [FromQuery] string? tags = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var data = await _vocabService.GetVocabularyListAsync(language, level, tags, page, pageSize);
            return Ok(new RepositoryModel<List<VocabularyModel>> { Data = data, StatusCode = 200 });
        }

        /// <summary>Lấy từ cần ôn hôm nay (SRS)</summary>
        [HttpGet("vocabulary/review")]
        [Authorize]
        public async Task<IActionResult> GetReviewItems(
            [FromQuery] string language,
            [FromQuery] int limit = 20)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var data = await _vocabService.GetReviewItemsAsync(userId, language, limit);
            return Ok(new RepositoryModel<List<VocabularyModel>> { Data = data, StatusCode = 200 });
        }

        /// <summary>Cập nhật tiến độ flashcard (SM-2)</summary>
        [HttpPost("vocabulary/{vocabId:int}/progress")]
        [Authorize]
        public async Task<IActionResult> UpdateProgress(int vocabId, [FromQuery] int quality)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();
            if (quality < 0 || quality > 5)
                return BadRequest(new RepositoryModel<object> { StatusCode = 400, SystemMessage = "quality phải từ 0-5" });

            await _vocabService.UpdateVocabProgressAsync(userId, vocabId, quality);
            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = 200 });
        }

        /// <summary>Thống kê tiến độ học từ vựng</summary>
        [HttpGet("vocabulary/stats")]
        [Authorize]
        public async Task<IActionResult> GetVocabStats([FromQuery] string language)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var stats = await _vocabService.GetVocabStatsAsync(userId, language);
            return Ok(new RepositoryModel<VocabStatsModel> { Data = stats, StatusCode = 200 });
        }

        // ── Helper ───────────────────────────────────────────────────────────
        private int GetUserId()
        {
            var claim = User.FindFirst("userId") ?? User.FindFirst("sub");
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }

        // ════════════════════════════════════════════════════════════════════
        // ADMIN ENDPOINTS (require Admin role)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>[ADMIN] Tạo đề thi mới</summary>
        [HttpPost("exams/admin")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminCreateExam([FromBody] CreateExamRequest model)
        {
            var result = await _examService.CreateExamAsync(model);
            return Ok(new RepositoryModel<LanguageExamModel> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Cập nhật đề thi</summary>
        [HttpPut("exams/admin/{examId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminUpdateExam(int examId, [FromBody] CreateExamRequest model)
        {
            var result = await _examService.UpdateExamAsync(examId, model);
            return Ok(new RepositoryModel<bool> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Xóa đề thi</summary>
        [HttpDelete("exams/admin/{examId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminDeleteExam(int examId)
        {
            var result = await _examService.DeleteExamAsync(examId);
            return Ok(new RepositoryModel<bool> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Lấy toàn bộ câu hỏi của đề thi (kèm đáp án đúng)</summary>
        [HttpGet("exams/{examId:int}/questions")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminGetQuestions(int examId)
        {
            var data = await _examService.GetQuestionsWithAnswersAsync(examId);
            return Ok(new RepositoryModel<List<ExamQuestionAdminModel>> { Data = data, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Thêm câu hỏi + đáp án vào phần thi</summary>
        [HttpPost("questions")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminCreateQuestion([FromBody] CreateQuestionRequest model)
        {
            var result = await _examService.CreateQuestionAsync(model);
            return Ok(new RepositoryModel<int> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Cập nhật câu hỏi</summary>
        [HttpPut("questions/{questionId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminUpdateQuestion(int questionId, [FromBody] CreateQuestionRequest model)
        {
            var result = await _examService.UpdateQuestionAsync(questionId, model);
            return Ok(new RepositoryModel<bool> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Xóa câu hỏi</summary>
        [HttpDelete("questions/{questionId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminDeleteQuestion(int questionId)
        {
            var result = await _examService.DeleteQuestionAsync(questionId);
            return Ok(new RepositoryModel<bool> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Tạo từ vựng mới</summary>
        [HttpPost("vocabulary/admin")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminCreateVocab([FromBody] CreateVocabRequest model)
        {
            var result = await _vocabService.CreateVocabAsync(model);
            return Ok(new RepositoryModel<int> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Cập nhật từ vựng</summary>
        [HttpPut("vocabulary/admin/{vocabId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminUpdateVocab(int vocabId, [FromBody] CreateVocabRequest model)
        {
            var result = await _vocabService.UpdateVocabAsync(vocabId, model);
            return Ok(new RepositoryModel<bool> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Xóa từ vựng</summary>
        [HttpDelete("vocabulary/admin/{vocabId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminDeleteVocab(int vocabId)
        {
            var result = await _vocabService.DeleteVocabAsync(vocabId);
            return Ok(new RepositoryModel<bool> { Data = result, StatusCode = 200 });
        }

        /// <summary>[ADMIN] Import hàng loạt từ vựng</summary>
        [HttpPost("vocabulary/bulk")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AdminBulkImportVocab([FromBody] BulkImportVocabRequest model)
        {
            var count = await _vocabService.BulkImportVocabAsync(model.Language, model.Items);
            return Ok(new RepositoryModel<int> { Data = count, StatusCode = 200, SystemMessage = $"Đã import {count} từ thành công." });
        }
    }
}
