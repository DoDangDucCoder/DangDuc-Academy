using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.Quiz)]
    public class QuizController : BaseController
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<RepositoryModel<QuizModel>> Create(QuizModel model)
        {
            return new RepositoryModel<QuizModel>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _quizService.CreateQuiz(model),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
        }

        [HttpGet("course/{idCourse}")]
        [AllowAnonymous]
        public async Task<RepositoryModel<List<QuizModel>>> GetByCourse(long idCourse)
        {
            return new RepositoryModel<List<QuizModel>>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _quizService.GetQuizzesByCourse(idCourse),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<RepositoryModel<QuizModel?>> Get(long id)
        {
            return new RepositoryModel<QuizModel?>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _quizService.GetQuiz(id),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<RepositoryModel<bool>> Update(long id, QuizModel model)
        {
            return new RepositoryModel<bool>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _quizService.UpdateQuiz(id, model),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<RepositoryModel<QuizAttemptResultModel>> Submit(SubmitQuizRequest request)
        {
            return new RepositoryModel<QuizAttemptResultModel>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _quizService.SubmitQuiz(request, CurrentUserId()),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
