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

        /// <summary>
        /// CourseController
        /// </summary>
        /// <param name="courseServices"></param>
        /// <param name="httpContextAccessor"></param>
        /// <exception cref="ProjectException"></exception>
        public CourseController(ICourseServices courseServices, IHttpContextAccessor httpContextAccessor)
        {
            _courseServices = courseServices ?? throw new ProjectException(nameof(_courseServices));
            _httpContextAccessor = httpContextAccessor ?? throw new ProjectException(nameof(_httpContextAccessor));
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

            result.Data = await _courseServices.GetListCourseForAdmin();
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
    }
}
