using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json.Serialization;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// CourseController
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.Course)]
    public class CourseController : BaseController
    {
        private readonly ICourseServices _courseServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly ICourseSearchIndexService _courseSearchIndexService;
        private readonly IAuditLogService _auditLogService;

        /// <summary>
        /// CourseController
        /// </summary>
        /// <param name="courseServices"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="cache"></param>
        /// <exception cref="ProjectException"></exception>
        public CourseController(ICourseServices courseServices, IHttpContextAccessor httpContextAccessor, IMemoryCache cache,
            ICourseSearchIndexService courseSearchIndexService, IAuditLogService auditLogService)
        {
            _courseServices = courseServices ?? throw new ProjectException(nameof(_courseServices));
            _httpContextAccessor = httpContextAccessor ?? throw new ProjectException(nameof(_httpContextAccessor));
            _cache = cache;
            _courseSearchIndexService = courseSearchIndexService;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// CreateCourse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("course")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse(CourseModel model)
        {
            RepositoryModel<CourseModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new CourseModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.CreateCourse(model);
            await _courseSearchIndexService.IndexCourse(result.Data);
            await _auditLogService.WriteAsync(CurrentUserId(), "Course.Create", "Course", result.Data.Id.ToString(), null, result.Data);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// UpdateStatusCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="processCourse"></param>
        /// <returns></returns>
        [HttpPut("course/status/{id}/{status}/{processCourse}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusCourse(long id, int status, int processCourse)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.UpdateStatusCourse(id, status, processCourse);
            await _courseSearchIndexService.DeleteCourse(id);
            await _auditLogService.WriteAsync(CurrentUserId(), "Course.UpdateStatus", "Course", id.ToString(), null, new { status, processCourse });
            return Ok(result);
        }

        /// <summary>
        /// UpdateInformationCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("course/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateInformationCourse(long id, CourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.UpdateInformationCourse(id, model);
            model.Id = id;
            await _courseSearchIndexService.IndexCourse(model);
            await _auditLogService.WriteAsync(CurrentUserId(), "Course.Update", "Course", id.ToString(), null, model);
            return Ok(result);
        }

        /// <summary>
        /// GetListAllCourse
        /// </summary>
        /// <returns></returns>
        [HttpGet("course")]
        public async Task<RepositoryModel<List<CourseModel>>> GetListAllCourse()
        {
            RepositoryModel<List<CourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<CourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
            var cacheKey = "courses_all";
            if (_cache.TryGetValue(cacheKey, out List<CourseModel>? cachedCourses) && cachedCourses != null)
            {
                result.Data = cachedCourses;
                return result;
            }

            result.Data = await _courseServices.GetListCourseForAdmin();
            _cache.Set(cacheKey, result.Data, TimeSpan.FromMinutes(5));
            return result;
        }

        /// <summary>
        /// GetListAllCoursePaged – Paginated version
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("course/paged/{pageIndex}/{pageSize}")]
        public async Task<RepositoryModel<PagedResult<CourseModel>>> GetListAllCoursePaged(int pageIndex, int pageSize)
        {
            RepositoryModel<PagedResult<CourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PagedResult<CourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.GetListCourseForAdminPaged(pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// SearchCourses – Advanced search with filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("course/search/{pageIndex}/{pageSize}")]
        [AllowAnonymous]
        public async Task<RepositoryModel<PagedResult<CourseModel>>> SearchCourses([FromQuery] HDNXUdemyModel.RequestModel.SearchCoursesRequest request, int pageIndex, int pageSize)
        {
            RepositoryModel<PagedResult<CourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PagedResult<CourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseSearchIndexService.SearchCourses(request, pageIndex, pageSize)
                ?? await _courseServices.SearchCourses(request, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Reindex all active courses into Elasticsearch.
        /// </summary>
        [HttpPost("course/search/reindex")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<int>> ReindexCourses()
        {
            RepositoryModel<int> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _courseSearchIndexService.ReindexAllCourses(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
            await _auditLogService.WriteAsync(CurrentUserId(), "Course.Reindex", "Course", null, null, result.Data);
            return result;
        }

        /// <summary>
        /// GetCourses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        [HttpGet("course/{id}/{isAdmin}")]
        public async Task<IActionResult> GetCourses(long id, bool isAdmin)
        {
            RepositoryModel<GetCourseWithDetailsContent> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new GetCourseWithDetailsContent(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetDetailCourse(id, isAdmin);
            if (result.Data?.Id == null)
            {
                result.RetCode = ERetCode.NoExitData;
                result.StatusCode = (int)HttpStatusCode.NotFound;
                result.SystemMessage = "Course not found.";
                return NotFound(result);
            }

            result.StatusCode = (int)HttpStatusCode.OK;
            return Ok(result);
        }

        /// <summary>
        /// GetListCourseAsCategory
        /// </summary>
        /// <param name="idCategory"></param>
        /// <returns></returns>
        [HttpGet("course/category/{idCategory}")]
        public async Task<RepositoryModel<List<CourseModel>>> GetListCourseAsCategory(long idCategory)
        {
            RepositoryModel<List<CourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<CourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetListCourseAsCategory(idCategory);
            return result;
        }

        /// <summary>
        /// GetListCourseAsCategoryPaged – Paginated version
        /// </summary>
        /// <param name="idCategory"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("course/category/{idCategory}/{pageIndex}/{pageSize}")]
        public async Task<RepositoryModel<PagedResult<CourseModel>>> GetListCourseAsCategoryPaged(long idCategory, int pageIndex, int pageSize)
        {
            RepositoryModel<PagedResult<CourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PagedResult<CourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.GetListCourseAsCategoryPaged(idCategory, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// GetListStudentProcess
        /// </summary>
        /// <returns></returns>
        [HttpGet("course/student-process")]
        public async Task<RepositoryModel<List<StudentProcessModel>>> GetListStudentProcess()
        {
            RepositoryModel<List<StudentProcessModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<StudentProcessModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetListStudentProcess();
            return result;
        }

        /// <summary>
        /// GetStudentProcess
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("course/student-process/{id}")]
        public async Task<RepositoryModel<StudentProcessModel>> GetStudentProcess(long id)
        {
            RepositoryModel<StudentProcessModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new StudentProcessModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetStudentProcess(id);
            return result;
        }

        /// <summary>
        /// AddCommentOfStudentForCourse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("course-comment")]
        [Authorize]
        public async Task<IActionResult> AddCommentOfStudentForCourse(CourseEvaluationModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = false,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            if (!IsAdmin())
            {
                var currentUserId = CurrentUserId();
                if (model.IdStudent != 0 && model.IdStudent != currentUserId)
                {
                    return Forbid();
                }

                model.IdStudent = currentUserId;
            }

            result.Data = await _courseServices.AddCommentOfStudentForCourse(model);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// GetListCoursEvaluation
        /// </summary>
        /// <param name="idCourse"></param>
        /// <returns></returns>
        [HttpGet("course-comment/course/{idCourse}")]
        public async Task<RepositoryModel<List<CourseEvaluationModel>>> GetListCourseEvaluation(long idCourse)
        {
            RepositoryModel<List<CourseEvaluationModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<CourseEvaluationModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetListCoursEvaluation(idCourse);
            return result;
        }

        /// <summary>
        /// UpdateStatusCommentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("course-comment/status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusCommentCourse(long id, CourseEvaluationModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.UpdateStatusCommentCourse(id, model);
            return Ok(result);
        }

        /// <summary>
        /// UpdateInformationCommentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("course-comment/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> UpdateInformationCommentCourse(long id, CourseEvaluationModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.UpdateInformationCommentCourse(id, model);
            return result;
        }

        /// <summary>
        /// GetCommentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("course-comment/{id}")]
        public async Task<RepositoryModel<CourseEvaluationModel>> GetCommentCourse(long id)
        {
            RepositoryModel<CourseEvaluationModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new CourseEvaluationModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetCommentCourse(id);
            return result;
        }

        /// <summary>
        /// CreateContentCourse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("course-content")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> CreateContentCourse(ContentCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.CreateContentCourse(model);
            return result;
        }

        /// <summary>
        /// UpdateStatusContentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("course-content/status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusContentCourse(long id, StatusUpdateRequest model)
        {
            if (model == null || !model.Status.HasValue)
            {
                throw new ProjectBadRequestException("Status is required.");
            }

            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.UpdateStatusContentCourse(id, new ContentCourseModel
            {
                Status = model.Status.Value
            });
            return Ok(result);
        }

        /// <summary>
        /// UpdateInformationContentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("course-content/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> UpdateInformationContentCourse(long id, ContentCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.UpdateInformationContentCourse(id, model);
            return result;
        }

        /// <summary>
        /// GetListContentCourse
        /// </summary>
        /// <returns></returns>
        [HttpGet("course-content/course/{idCourse}")]
        public async Task<RepositoryModel<List<ListContentWithDetailCourse>>> GetListContentCourse(long idCourse)
        {
            RepositoryModel<List<ListContentWithDetailCourse>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<ListContentWithDetailCourse>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetListContentCourse(idCourse);
            return result;
        }

        /// <summary>
        /// GetContentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("course-content/{id}")]
        public async Task<RepositoryModel<ContentCourseModel>> GetContentCourse(long id)
        {
            RepositoryModel<ContentCourseModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ContentCourseModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetContentCourse(id);
            return result;
        }

        /// <summary>
        /// CreateContentCourseDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("course-content-details")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> CreateContentCourseDetails(ContentCourseDetailModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.CreateContentCourseDetails(model);
            return result;
        }

        /// <summary>
        /// UpdateStatusContentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("course-content-details/status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusContentCourseDetails(long id, ContentCourseDetailModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.UpdateStatusContentCourseDetails(id, model);
            return Ok(result);
        }

        /// <summary>
        /// UpdateInformationContentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("course-content-details/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> UpdateInformationContentCourseDetails(long id, ContentCourseDetailModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.UpdateInformationContentCourseDetails(id, model);
            return result;
        }

        /// <summary>
        /// GetListContentCourseDetails
        /// </summary>
        /// <returns></returns>
        [HttpGet("course-content-details/course/{idCourse}")]
        public async Task<RepositoryModel<List<ContentCourseDetailModel>>> GetListContentCourseDetails(long idCourse)
        {
            RepositoryModel<List<ContentCourseDetailModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<ContentCourseDetailModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetListContentCourseDetails(idCourse);
            return result;
        }

        /// <summary>
        /// GetContentCourseDetails
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("course-content-details/{id}")]
        public async Task<RepositoryModel<ContentCourseDetailModel>> GetContentCourseDetails(long id)
        {
            RepositoryModel<ContentCourseDetailModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ContentCourseDetailModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.GetContentCourseDetails(id, Request);
            return result;
        }

        /// <summary>
        /// CreateTheadQuestionCourse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("thead-question-course")]
        [Authorize]
        public async Task<RepositoryModel<bool>> CreateTheadQuestionCourse(TheadQuestionCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.CreateTheadQuestionCourse(model);
            return result;
        }

        /// <summary>
        /// UpdateStatusTheadQuestionCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("thead-question-course/status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusTheadQuestionCourse(long id, TheadQuestionCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.UpdateStatusTheadQuestionCourse(id, model);
            return Ok(result);
        }

        /// <summary>
        /// UpdateInformationTheadQuestionCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("thead-question-course/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> UpdateInformationTheadQuestionCourse(long id, TheadQuestionCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.UpdateInformationTheadQuestionCourse(id, model);
            return result;
        }

        /// <summary>
        /// GetListCommentChapter
        /// </summary>
        /// <returns></returns>
        [HttpGet("thead-question-course/course/{idCourse}")]
        public async Task<RepositoryModel<List<TheadQuestionCourseModel>>> GetListTheadQuestionCourse(long idCourse)
        {
            RepositoryModel<List<TheadQuestionCourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<TheadQuestionCourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetListTheadQuestionCourse(idCourse);
            return result;
        }

        /// <summary>
        /// GetTheadQuestionCourse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("thead-question-course/{id}")]
        public async Task<RepositoryModel<TheadQuestionCourseModel>> GetTheadQuestionCourse(long id)
        {
            RepositoryModel<TheadQuestionCourseModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new TheadQuestionCourseModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetTheadQuestionCourse(id);
            return result;
        }

        /// <summary>
        /// LikeForCommentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("course-comment-like/{id}")]
        [Authorize]
        public async Task<RepositoryModel<bool>> LikeForCommentCourse(long id)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = false,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.LikeForCommentCourse(id);
            return result;
        }

        /// <summary>
        /// DisLikeForCommentCourse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("course-comment-dislike/{id}")]
        [Authorize]
        public async Task<RepositoryModel<bool>> DisLikeForCommentCourse(long id)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = false,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.DisLikeForCommentCourse(id);
            return result;
        }

        /// <summary>
        /// CreateDetailsTheadQuestionCourse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("details-thead-question-course")]
        [Authorize]
        public async Task<RepositoryModel<bool>> CreateDetailsTheadQuestionCourse(DetailTheadQuestionCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.CreateDetailsTheadQuestionCourse(model);
            return result;
        }

        /// <summary>
        /// UpdateStatusDetailsTheadQuestionCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("details-thead-question-course/status/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusDetailsTheadQuestionCourse(long id, DetailTheadQuestionCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _courseServices.UpdateStatusDetailsTheadQuestionCourse(id, model);
            return Ok(result);
        }

        /// <summary>
        /// UpdateInformationDetailsTheadQuestionCourse
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("details-thead-question-course/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> UpdateInformationDetailsTheadQuestionCourse(long id, DetailTheadQuestionCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.UpdateInformationDetailsTheadQuestionCourse(id, model);
            return result;
        }

        /// <summary>
        /// GetListDetailsTheadQuestionCourse
        /// </summary>
        /// <returns></returns>
        [HttpGet("details-thead-question-course/course/{idThead}")]
        public async Task<RepositoryModel<List<DetailTheadQuestionCourseModel>>> GetListDetailsTheadQuestionCourse(long idThead)
        {
            RepositoryModel<List<DetailTheadQuestionCourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new List<DetailTheadQuestionCourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _courseServices.GetListDetailsTheadQuestionCourse(idThead);
            return result;
        }

        public sealed class StatusUpdateRequest
        {
            [JsonPropertyName("status")]
            public int? Status { get; set; }
        }

        /// <summary>
        /// ExportCoursesToExcel – Download courses as Excel file
        /// </summary>
        /// <returns></returns>
        [HttpGet("course/export/excel")]
        public async Task<IActionResult> ExportCoursesToExcel()
        {
            var courses = await _courseServices.GetListCourseForAdmin();
            var exportData = courses.Select(c => new
            {
                c.Id,
                Title = c.Title ?? "",
                Category = c.CategoryName ?? "",
                Author = c.UserName ?? "",
                Price = c.PriceOfCourse,
                DiscountPrice = c.PriceOfDiscount,
                IsFree = c.IsFree,
                Level = c.LevelCourse ?? "",
                Rating = c.AverageScore,
                Status = c.Status
            }).ToList();

            var bytes = Helpers.ExcelExportHelper.ExportToExcel(exportData, "Courses");
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"courses_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }
    }
}
