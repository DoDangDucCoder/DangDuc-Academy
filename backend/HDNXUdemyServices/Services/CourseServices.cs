using AutoMapper;
using DNXUdemyData.Entities;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.CommonFunction;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace HDNXUdemyServices.Services
{
    public class CourseServices : ICourseServices
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IContentCourseRepository _contentCourseRepository;
        private readonly IContentCourseDetailRepository _contentCourseDetailRepository;
        private readonly IRTheadQuestionCourseRepository _theadQuestionCourseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPurcharseCourseRepository _pucharseCourseRepository;
        private readonly IRPPurcharseCourseDetailsRepository _purcharseCourseDetailsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRCourseEvaluationRepository _courseEvaluationRepository;
        private readonly IRDetailTheadQuestionCourseRepository _detailsTheadQuestionCourseRepository;

        public CourseServices(ICourseRepository courseRepository, IContentCourseRepository contentCourseRepository, IContentCourseDetailRepository contentCourseDetailRepository,
            IRTheadQuestionCourseRepository theadQuestionCourseRepository, IMapper mapper, ICategoryRepository categoryRepository,
            IUserRepository userRepository, IPurcharseCourseRepository pucharseCourseRepository, IRPPurcharseCourseDetailsRepository purcharseCourseDetailsRepository,
            IHttpContextAccessor httpContextAccessor, IRCourseEvaluationRepository courseEvaluationRepository, IRDetailTheadQuestionCourseRepository detailsTheadQuestionCourseRepository)
        {
            _courseRepository = courseRepository ?? throw new ProjectException(nameof(_courseRepository));
            _contentCourseRepository = contentCourseRepository ?? throw new ProjectException(nameof(_contentCourseRepository));
            _contentCourseDetailRepository = contentCourseDetailRepository ?? throw new ProjectException(nameof(_contentCourseDetailRepository));
            _theadQuestionCourseRepository = theadQuestionCourseRepository ?? throw new ProjectException(nameof(_theadQuestionCourseRepository));
            _categoryRepository = categoryRepository ?? throw new ProjectException(nameof(_categoryRepository));
            _userRepository = userRepository ?? throw new ProjectException(nameof(_userRepository));
            _mapper = mapper ?? throw new ProjectException(nameof(_mapper));
            _pucharseCourseRepository = pucharseCourseRepository ?? throw new ProjectException(nameof(_pucharseCourseRepository));
            _purcharseCourseDetailsRepository = purcharseCourseDetailsRepository ?? throw new ProjectException(nameof(_purcharseCourseDetailsRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ProjectException(nameof(_httpContextAccessor));
            _courseEvaluationRepository = courseEvaluationRepository ?? throw new ProjectException(nameof(_courseEvaluationRepository));
            _detailsTheadQuestionCourseRepository = detailsTheadQuestionCourseRepository ?? throw new ProjectException(nameof(_detailsTheadQuestionCourseRepository)); ;
        }

        public async Task<CourseModel> CreateCourse(CourseModel model)
        {
            ValidateCourse(model);
            await ValidateCourseTitleIsUnique(model.Title!, null);
            var dataInsert = _mapper.Map<CourseEntities>(model);
            ApplyCourseUpdate(dataInsert, model);
            var dataAfterInsert = _mapper.Map<CourseModel>(await _courseRepository.AddReturnModelAsync(dataInsert));
            return dataAfterInsert;
        }

        public async Task<bool> UpdateStatusCourse(long id, int status, int processCourse)
        {
            ValidateId(id);
            ValidateStatus(status);
            if (!Enum.IsDefined(typeof(ProcessVideo), processCourse))
            {
                throw new ProjectBadRequestException("Process course is invalid.");
            }

            var getData = await _courseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course not found.");
            getData.Status = status;
            getData.ProcessCourse = processCourse;
            return await _courseRepository.UpdateAsync(getData);
        }

        public async Task<bool> UpdateInformationCourse(long id, CourseModel model)
        {
            ValidateId(id);
            ValidateCourse(model);

            var getData = await _courseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course not found.");
            await ValidateCourseTitleIsUnique(model.Title!, id);

            ApplyCourseUpdate(getData, model);
            return await _courseRepository.UpdateAsync(getData);
        }

        public async Task<List<CourseModel>> GetListCourseForAdmin()
        {
            var getData = await _courseRepository.GetAllAsync();
            var getCategory = await _categoryRepository.GetAllAsync();
            var resultMapping = _mapper.Map<List<CourseModel>>(getData);
            foreach (var item in resultMapping)
            {
                var getDataOfVoteStar = await _courseEvaluationRepository.GetAsync(x => x.IdCourse == item.Id);
                var getVoteData = HelperFunction.CalculatorToTalStartOfCourse(getDataOfVoteStar.ToList());
                item.TotalVoteOfCourse = getDataOfVoteStar.Count();
                item.Vote1Star = getVoteData.Item1;
                item.Vote2Star = getVoteData.Item2;
                item.Vote3Star = getVoteData.Item3;
                item.Vote4Star = getVoteData.Item4;
                item.Vote5Star = getVoteData.Item5;
                item.AverageScore = getVoteData.Item6;
                item.CategoryName = getCategory.Where(x => x.Id == item.IdCategory).FirstOrDefault()?.Name;
                item.UserName = "Admin";
                item.ProcessCourseName = item.ProcessCourse.HasValue
                    ? ((ProcessVideo)item.ProcessCourse.Value).GetEnumDescription()
                    : null;
            }
            return resultMapping;
        }

        public async Task<List<CourseModel>> GetListCourseOfStudent(long idStudent)
        {
            var getIdOfPurchaseOrder = await _pucharseCourseRepository.
                GetAsync(x => x.IdStudent == idStudent && x.PurcharseStatus == (int)ETypeOfStatusOrder.Payment && x.Status == (int)EStatus.Active);
            var getIdOfCourseOfStudent = await _purcharseCourseDetailsRepository.GetAsync(item => getIdOfPurchaseOrder.Select(x => x.Id).Contains(item.IdPurchaseOrder));
            var getData = await _courseRepository.GetAsync(item => getIdOfCourseOfStudent.Select(x => x.IdCourse).Contains(item.Id));
            var getCategory = await _categoryRepository.GetAllAsync();
            var resultMapping = _mapper.Map<List<CourseModel>>(getData);
            foreach (var item in resultMapping)
            {
                var getDataOfVoteStar = await _courseEvaluationRepository.GetAsync(x => x.IdCourse == item.Id);
                var getVoteData = HelperFunction.CalculatorToTalStartOfCourse(getDataOfVoteStar.ToList());
                item.TotalVoteOfCourse = getDataOfVoteStar.Count();
                item.Vote1Star = getVoteData.Item1;
                item.Vote2Star = getVoteData.Item2;
                item.Vote3Star = getVoteData.Item3;
                item.Vote4Star = getVoteData.Item4;
                item.Vote5Star = getVoteData.Item5;
                item.AverageScore = getVoteData.Item6;
                item.CategoryName = getCategory.Where(x => x.Id == item.IdCategory).FirstOrDefault()?.Name;
                item.UserName = (await _userRepository.GetByIdAsync(item.CreateBy))?.Name;
            }
            return resultMapping;
        }

        public async Task<GetCourseWithDetailsContent> GetDetailCourse(long id, bool isAdmin)
        {
            var getData = await _courseRepository.GetByIdAsync(id);
            var resultMapping = _mapper.Map<GetCourseWithDetailsContent>(getData);
            long idCurrentUser = _httpContextAccessor.HttpContext == null ? 0 : long.Parse(_httpContextAccessor.HttpContext.User.Claims
                                .Where(x => x.Type == "user-id").FirstOrDefault()?.Value ?? "0");
            bool isPurchase = false;
            if (isAdmin)
            {
                isAdmin = true;
            }
            else
            {
                var isPurchaseCourseDetails = await _purcharseCourseDetailsRepository.GetAsync(x => x.IdCourse == id && x.IdStudent == idCurrentUser);

                if (isPurchaseCourseDetails.Any())
                {
                    var purchaseDetail = isPurchaseCourseDetails.FirstOrDefault();
                    if (purchaseDetail != null)
                    {
                        var isPurchaseCourse = await _pucharseCourseRepository.GetByIdAsync(purchaseDetail.IdPurchaseOrder);
                        isPurchase = isAdmin = isPurchaseCourse?.PurcharseStatus == (int)ETypeOfStatusOrder.Payment;
                    }
                }
            }

            if (getData != null && resultMapping != null)
            {
                var resultCourse = _mapper.Map<List<CourseModel>>((await _courseRepository.GetAsync(x => x.IdCategory == getData.IdCategory)).Take(3));
                var resultAuthor = _mapper.Map<UserModel>(await _userRepository.GetObjectAsync(x => x.Id == getData.CreateBy));
                var getCategory = await _categoryRepository.GetAllAsync();
                var getDataOfVoteStar = await _courseEvaluationRepository.GetAsync(x => x.IdCourse == resultMapping.Id);
                var getVoteData = HelperFunction.CalculatorToTalStartOfCourse(getDataOfVoteStar.ToList());
                resultMapping.TotalVoteOfCourse = getDataOfVoteStar.Count();
                resultMapping.Vote1Star = getVoteData.Item1;
                resultMapping.Vote2Star = getVoteData.Item2;
                resultMapping.Vote3Star = getVoteData.Item3;
                resultMapping.Vote4Star = getVoteData.Item4;
                resultMapping.Vote5Star = getVoteData.Item5;
                resultMapping.AverageScore = getVoteData.Item6;
                resultMapping.CategoryName = getCategory.Where(x => x.Id == resultMapping.IdCategory).FirstOrDefault()?.Name;
                resultMapping.UserName = (await _userRepository.GetByIdAsync(resultMapping.CreateBy))?.Name;
                resultMapping.ProcessCourseName = resultMapping.ProcessCourse.HasValue
                    ? ((ProcessVideo)resultMapping.ProcessCourse.Value).GetEnumDescription()
                    : null;
                resultMapping.ListContentCourseDetails = await GetContentOfCourse(getData.Id, isAdmin);
                resultMapping.ListCourseRate = resultCourse;
                resultMapping.Author = resultAuthor;
                resultMapping.FileUploadUrlStream = $"{ProjectConfig.APIUrlGetVideoMp4}{resultMapping.FileUrl}";
                resultMapping.IsPurchase = isPurchase;
            }

            return resultMapping ?? new GetCourseWithDetailsContent();
        }

        public async Task<List<StudentProcessModel>> GetListStudentProcess()
        {
            var getData = await _courseRepository.GetAllAsync();
            return _mapper.Map<List<StudentProcessModel>>(getData);
        }

        public async Task<StudentProcessModel> GetStudentProcess(long id)
        {
            var getData = await _courseRepository.GetByIdAsync(id);
            return _mapper.Map<StudentProcessModel>(getData);
        }

        public async Task<bool> AddCommentOfStudentForCourse(CourseEvaluationModel model)
        {
            if (model.IdStudent <= 0)
            {
                throw new ProjectBadRequestException("Student is required.");
            }

            var course = await _courseRepository.GetObjectAsync(x => x.Id == model.IdCourse && x.Status == (int)EStatus.Active);
            if (course == null)
            {
                throw new ProjectBadRequestException("Course is invalid.");
            }

            if (model.VoteStartNumber < 1 || model.VoteStartNumber > 5)
            {
                throw new ProjectBadRequestException("Vote must be between 1 and 5.");
            }

            if (string.IsNullOrWhiteSpace(model.CommentEvaluation))
            {
                throw new ProjectBadRequestException("Comment is required.");
            }

            model.CommentEvaluation = model.CommentEvaluation.Trim();
            if (model.CommentEvaluation.Length > 1000)
            {
                throw new ProjectBadRequestException("Comment must be 1000 characters or fewer.");
            }

            if (Regex.IsMatch(model.CommentEvaluation, @"<\s*/?\s*[a-zA-Z][^>]*>", RegexOptions.CultureInvariant))
            {
                throw new ProjectBadRequestException("Comment cannot contain HTML or script content.");
            }

            var insertData = _mapper.Map<CourseEvaluationEntities>(model);
            return await _courseEvaluationRepository.AddAsync(insertData);
        }

        public async Task<List<CourseEvaluationModel>> GetListCoursEvaluation(long idCourse)
        {
            var getData = await _courseEvaluationRepository.GetAsync(x => x.IdCourse == idCourse && x.Status == (int)EStatus.Active);
            var returnData = _mapper.Map<List<CourseEvaluationModel>>(getData);
            foreach (var item in returnData)
            {
                item.Users = _mapper.Map<UserModel>(await _userRepository.GetByIdAsync(item.CreateBy));
            }
            return returnData;
        }

        public async Task<bool> UpdateStatusCommentCourse(long id, CourseEvaluationModel model)
        {
            ValidateId(id);
            ValidateStatus(model?.Status ?? -1);
            var getData = await _courseEvaluationRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course comment not found.");
            getData.Status = model.Status;
            return await _courseEvaluationRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateInformationCommentCourse(long id, CourseEvaluationModel model)
        {
            ValidateId(id);
            ValidateComment(model);
            var getData = await _courseEvaluationRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course comment not found.");
            var dataInsert = _mapper.Map<CourseEvaluationEntities>(model);
            dataInsert.CreateDate = getData.CreateDate;
            return await _courseEvaluationRepository.UpdateAsync(dataInsert);
        }

        public async Task<CourseEvaluationModel> GetCommentCourse(long id)
        {
            var getData = await _courseEvaluationRepository.GetByIdAsync(id);
            return _mapper.Map<CourseEvaluationModel>(getData);
        }

        public async Task<bool> CreateContentCourse(ContentCourseModel model)
        {
            ValidateContentCourse(model);
            var dataInsert = _mapper.Map<ContentCourseEntities>(model);
            return await _contentCourseRepository.AddAsync(dataInsert);
        }

        public async Task<bool> UpdateStatusContentCourse(long id, ContentCourseModel model)
        {
            ValidateId(id);
            ValidateStatus(model?.Status ?? -1);
            var getData = await _contentCourseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course content not found.");
            getData.Status = model.Status;
            return await _contentCourseRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateInformationContentCourse(long id, ContentCourseModel model)
        {
            ValidateId(id);
            ValidateContentCourse(model);
            var getData = await _contentCourseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course content not found.");
            var dataInsert = _mapper.Map<ContentCourseEntities>(model);
            dataInsert.CreateDate = getData.CreateDate;
            return await _contentCourseRepository.UpdateAsync(dataInsert);
        }

        public async Task<List<ListContentWithDetailCourse>> GetListContentCourse(long idCourse)
        {
            var getData = await _contentCourseRepository.GetAsync(x => x.IdCourse == idCourse);
            var mappingDataContentCourse = _mapper.Map<List<ListContentWithDetailCourse>>(getData.OrderBy(x => x.CreateDate));
            foreach (var item in mappingDataContentCourse)
            {
                var getDataCourseDetail = await _contentCourseDetailRepository.GetAsync(x => x.IdContent == item.Id);
                item.ListContentCourseDetails = _mapper.Map<List<ContentCourseDetailModel>>(getDataCourseDetail);
            }

            return mappingDataContentCourse;
        }

        public async Task<ContentCourseModel> GetContentCourse(long id)
        {
            var getData = await _contentCourseRepository.GetByIdAsync(id);
            return _mapper.Map<ContentCourseModel>(getData);
        }

        public async Task<bool> CreateContentCourseDetails(ContentCourseDetailModel model)
        {
            ValidateContentCourseDetail(model);
            var dataInsert = _mapper.Map<ContentCourseDetailEntities>(model);
            return await _contentCourseDetailRepository.AddAsync(dataInsert);
        }

        public async Task<bool> UpdateStatusContentCourseDetails(long id, ContentCourseDetailModel model)
        {
            ValidateId(id);
            ValidateStatus(model?.Status ?? -1);
            var getData = await _contentCourseDetailRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course content detail not found.");
            getData.Status = model.Status;
            return await _contentCourseDetailRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateInformationContentCourseDetails(long id, ContentCourseDetailModel model)
        {
            ValidateId(id);
            ValidateContentCourseDetail(model);
            var getData = await _contentCourseDetailRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Course content detail not found.");
            var dataInsert = _mapper.Map<ContentCourseDetailEntities>(model);
            dataInsert.CreateDate = getData.CreateDate;
            return await _contentCourseDetailRepository.UpdateAsync(dataInsert);
        }

        public async Task<List<ContentCourseDetailModel>> GetListContentCourseDetails(long idContent)
        {
            var getData = await _contentCourseDetailRepository.GetAsync(x => x.IdContent == idContent);
            return _mapper.Map<List<ContentCourseDetailModel>>(getData);
        }

        public async Task<ContentCourseDetailModel> GetContentCourseDetails(long id, HttpRequest request)
        {
            var getData = await _contentCourseDetailRepository.GetByIdAsync(id);
            var resultMapping = _mapper.Map<ContentCourseDetailModel>(getData) ?? new ContentCourseDetailModel();
            if (getData == null)
            {
                return resultMapping;
            }

            var content = await _contentCourseRepository.GetByIdAsync(getData.IdContent);
            var course = content == null ? null : await _courseRepository.GetByIdAsync(content.IdCourse);
            var currentUserId = GetCurrentUserId();
            var canAccessPrivateContent = resultMapping.IsLearningFree || IsCurrentUserAdmin() || (course?.IsFree == true);

            if (!canAccessPrivateContent && content != null && currentUserId > 0)
            {
                var purchaseDetails = await _purcharseCourseDetailsRepository.GetAsync(x => x.IdCourse == content.IdCourse && x.IdStudent == currentUserId);
                foreach (var purchaseDetail in purchaseDetails)
                {
                    var purchaseOrder = await _pucharseCourseRepository.GetByIdAsync(purchaseDetail.IdPurchaseOrder);
                    if (purchaseOrder?.PurcharseStatus == (int)ETypeOfStatusOrder.Payment && purchaseOrder.Status == (int)EStatus.Active)
                    {
                        canAccessPrivateContent = true;
                        break;
                    }
                }
            }

            resultMapping.FileUploadUrlStream = canAccessPrivateContent && !string.IsNullOrWhiteSpace(resultMapping.FileNameVideo)
                ? $"{ProjectConfig.APIUrlGetVideoStream}{resultMapping.FileNameVideo}"
                : null;
            return resultMapping;
        }

        private long GetCurrentUserId()
        {
            var rawUserId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "user-id")?.Value;
            return long.TryParse(rawUserId, out var userId) ? userId : 0;
        }

        private bool IsCurrentUserAdmin()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(ERoles.Admin.ToString()) == true;
        }

        public async Task<List<ContentAndContentDetail>> GetContentOfCourse(long idCourse, bool isAdmin)
        {
            var resultData = new List<ContentAndContentDetail>();
            var getDataCourseContent = await _contentCourseRepository.GetAsync(x => x.IdCourse == idCourse);
            resultData = _mapper.Map<List<ContentAndContentDetail>>(getDataCourseContent.OrderBy(x => x.CreateDate));

            foreach (var item in resultData)
            {
                item.ContentAndContentDetails = new List<ContentCourseDetailModel>();
                if (isAdmin)
                {
                    item.ContentAndContentDetails = _mapper.Map<List<ContentCourseDetailModel>>(await _contentCourseDetailRepository.GetAsync(x => x.IdContent == item.Id));
                }
                else
                {
                    item.ContentAndContentDetails = _mapper.Map<List<ContentCourseDetailModel>>(await _contentCourseDetailRepository.GetAsync(x => x.IdContent == item.Id));
                    item.ContentAndContentDetails.ForEach(item =>
                    {
                        if (!item.IsLearningFree)
                        {
                            item.FileUploadUrlStream = null;
                        }
                    });
                }
            }

            return resultData;
        }

        public async Task<bool> CreateTheadQuestionCourse(TheadQuestionCourseModel model)
        {
            ValidateThread(model);
            var dataInsert = _mapper.Map<TheadQuestionCourseEntities>(model);
            return await _theadQuestionCourseRepository.AddAsync(dataInsert);
        }

        public async Task<bool> UpdateStatusTheadQuestionCourse(long id, TheadQuestionCourseModel model)
        {
            ValidateId(id);
            ValidateStatus(model?.Status ?? -1);
            var getData = await _theadQuestionCourseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Thread question not found.");
            getData.Status = model.Status;
            return await _theadQuestionCourseRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateInformationTheadQuestionCourse(long id, TheadQuestionCourseModel model)
        {
            ValidateId(id);
            ValidateThread(model);
            var getData = await _theadQuestionCourseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Thread question not found.");
            var dataInsert = _mapper.Map<TheadQuestionCourseEntities>(model);
            dataInsert.CreateDate = getData.CreateDate;
            return await _theadQuestionCourseRepository.UpdateAsync(dataInsert);
        }

        public async Task<List<TheadQuestionCourseModel>> GetListTheadQuestionCourse(long idCourse)
        {
            var getData = await _theadQuestionCourseRepository.GetAsync(x => x.IdCourse == idCourse);
            return _mapper.Map<List<TheadQuestionCourseModel>>(getData);
        }

        public async Task<TheadQuestionCourseModel> GetTheadQuestionCourse(long id)
        {
            var getData = await _theadQuestionCourseRepository.GetByIdAsync(id);
            return _mapper.Map<TheadQuestionCourseModel>(getData);
        }

        public async Task<bool> CreateDetailsTheadQuestionCourse(DetailTheadQuestionCourseModel model)
        {
            ValidateThreadDetail(model);
            var dataInsert = _mapper.Map<DetailTheadQuestionCourseEntities>(model);
            return await _detailsTheadQuestionCourseRepository.AddAsync(dataInsert);
        }

        public async Task<bool> UpdateStatusDetailsTheadQuestionCourse(long id, DetailTheadQuestionCourseModel model)
        {
            ValidateId(id);
            ValidateStatus(model?.Status ?? -1);
            var getData = await _detailsTheadQuestionCourseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Thread question detail not found.");
            getData.Status = model.Status;
            return await _detailsTheadQuestionCourseRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateInformationDetailsTheadQuestionCourse(long id, DetailTheadQuestionCourseModel model)
        {
            ValidateId(id);
            ValidateThreadDetail(model);
            var getData = await _detailsTheadQuestionCourseRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Thread question detail not found.");
            var dataInsert = _mapper.Map<DetailTheadQuestionCourseEntities>(model);
            dataInsert.CreateDate = getData.CreateDate;
            return await _detailsTheadQuestionCourseRepository.UpdateAsync(dataInsert);
        }

        public async Task<List<DetailTheadQuestionCourseModel>> GetListDetailsTheadQuestionCourse(long idThear)
        {
            var getData = await _detailsTheadQuestionCourseRepository.GetAsync(x => x.IdTheadQuestionCourse == idThear);
            return _mapper.Map<List<DetailTheadQuestionCourseModel>>(getData);
        }

        public async Task<List<CourseModel>> GetListCourseAsCategory(long idCategory)
        {
            var getData = await _courseRepository.GetAsync(x => x.IdCategory == idCategory);
            return _mapper.Map<List<CourseModel>>(getData);
        }

        private static void ValidateId(long id)
        {
            if (id <= 0)
            {
                throw new ProjectBadRequestException("Id is invalid.");
            }
        }

        private static void ValidateStatus(int status)
        {
            if (!Enum.IsDefined(typeof(EStatus), status))
            {
                throw new ProjectBadRequestException("Status is invalid.");
            }
        }

        private static void ValidateText(string? value, string fieldName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProjectBadRequestException($"{fieldName} is required.");
            }

            var text = value.Trim();
            if (text.Length > maxLength)
            {
                throw new ProjectBadRequestException($"{fieldName} is too long.");
            }

            if (Regex.IsMatch(text, @"<\s*/?\s*[a-zA-Z][^>]*>", RegexOptions.CultureInvariant))
            {
                throw new ProjectBadRequestException($"{fieldName} cannot contain HTML or script content.");
            }
        }

        private static void ValidateOptionalText(string? value, string fieldName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            ValidateText(value, fieldName, maxLength);
        }

        private static void ValidateRichText(string? value, string fieldName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProjectBadRequestException($"{fieldName} is required.");
            }

            var text = value.Trim();
            if (text.Length > maxLength)
            {
                throw new ProjectBadRequestException($"{fieldName} is too long.");
            }

            if (Regex.IsMatch(text, @"<\s*/?\s*script\b|on\w+\s*=|javascript\s*:", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                throw new ProjectBadRequestException($"{fieldName} cannot contain script content.");
            }
        }

        private static void ValidateCourse(CourseModel model)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            ValidateText(model.Title, nameof(model.Title), 255);
            ValidateText(model.ShortDescription, nameof(model.ShortDescription), 1000);
            ValidateText(model.Introduce, nameof(model.Introduce), 2000);
            ValidateText(model.LevelCourse, nameof(model.LevelCourse), 100);
            ValidateRichText(model.Description, nameof(model.Description), 5000);
            ValidateOptionalText(model.Duration, nameof(model.Duration), 100);
            ValidateOptionalText(model.PublicId, nameof(model.PublicId), 255);
            ValidateOptionalText(model.PictureUrl, nameof(model.PictureUrl), 2000);
            ValidateOptionalText(model.KeyVideoUpload, nameof(model.KeyVideoUpload), 500);
            ValidateOptionalText(model.FileUrl, nameof(model.FileUrl), 2000);

            if (model.IdAuthor <= 0)
            {
                throw new ProjectBadRequestException("Author is required.");
            }

            if (!model.IdCategory.HasValue || model.IdCategory <= 0)
            {
                throw new ProjectBadRequestException("Category is required.");
            }

            if (!model.PriceOfCourse.HasValue || model.PriceOfCourse < 0)
            {
                throw new ProjectBadRequestException("Price is required.");
            }

            if (!model.IsDiscount.HasValue || !model.PriceOfDiscount.HasValue || model.PriceOfDiscount < 0)
            {
                throw new ProjectBadRequestException("Discount information is required.");
            }

            if (!model.IsFree.HasValue)
            {
                throw new ProjectBadRequestException("Free course flag is required.");
            }

            if (!model.TypeOfCourse.HasValue || !Enum.IsDefined(typeof(ETypeOfCourse), model.TypeOfCourse.Value))
            {
                throw new ProjectBadRequestException("Type of course is invalid.");
            }

            if (!model.Languages.HasValue)
            {
                throw new ProjectBadRequestException("Language is required.");
            }

            if (model.TotalStudentRegister.HasValue && model.TotalStudentRegister < 0)
            {
                throw new ProjectBadRequestException("Total student register is invalid.");
            }

            if (model.TotalChapter.HasValue && model.TotalChapter < 0)
            {
                throw new ProjectBadRequestException("Total chapter is invalid.");
            }

            ValidateStatus(model.Status);

            if (model.ProcessCourse.HasValue && !Enum.IsDefined(typeof(ProcessVideo), model.ProcessCourse.Value))
            {
                throw new ProjectBadRequestException("Process course is invalid.");
            }
        }

        private async Task ValidateCourseTitleIsUnique(string title, long? excludeId)
        {
            var normalizedTitle = title.Trim().ToLowerInvariant();
            var duplicateCourses = await _courseRepository.GetAsync(x =>
                x.Title != null &&
                x.Title.ToLower() == normalizedTitle &&
                x.Status == (int)EStatus.Active &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (duplicateCourses.Any())
            {
                throw new ProjectConflictException("Course title already exists.");
            }
        }

        private static void ApplyCourseUpdate(CourseEntities entity, CourseModel model)
        {
            entity.Title = model.Title!.Trim();
            entity.ShortDescription = model.ShortDescription!.Trim();
            entity.IdAuthor = model.IdAuthor;
            entity.TotalStudentRegister = model.TotalStudentRegister ?? entity.TotalStudentRegister;
            entity.TotalChapter = model.TotalChapter ?? entity.TotalChapter;
            entity.Duration = model.Duration?.Trim();
            entity.PriceOfCourse = model.PriceOfCourse!.Value;
            entity.IsDiscount = model.IsDiscount!.Value;
            entity.PriceOfDiscount = model.PriceOfDiscount!.Value;
            entity.IsFree = model.IsFree!.Value;
            entity.TypeOfCourse = model.TypeOfCourse!.Value;
            entity.Introduce = model.Introduce!.Trim();
            entity.IdCategory = model.IdCategory!.Value;
            entity.LevelCourse = model.LevelCourse!.Trim();
            entity.Languages = model.Languages!.Value;
            entity.Description = model.Description!.Trim();
            entity.PublicId = model.PublicId?.Trim();
            entity.PictureUrl = model.PictureUrl?.Trim();
            entity.KeyVideoUpload = model.KeyVideoUpload?.Trim();
            entity.FileUrl = model.FileUrl?.Trim();
            entity.ProcessCourse = model.ProcessCourse;
            entity.Status = model.Status;
        }

        private static void ValidateContentCourse(ContentCourseModel model)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (model.IdCourse <= 0)
            {
                throw new ProjectBadRequestException("Course is required.");
            }

            ValidateText(model.Name, nameof(model.Name), 255);
            ValidateStatus(model.Status);
        }

        private static void ValidateContentCourseDetail(ContentCourseDetailModel model)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (model.IdContent <= 0)
            {
                throw new ProjectBadRequestException("Content is required.");
            }

            ValidateText(model.NameSubContent, nameof(model.NameSubContent), 255);
            ValidateStatus(model.Status);
        }

        private static void ValidateThread(TheadQuestionCourseModel model)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (model.IdCourse <= 0 || model.IdStudent <= 0)
            {
                throw new ProjectBadRequestException("Course and student are required.");
            }

            ValidateText(model.Comment, nameof(model.Comment), 1000);
            ValidateStatus(model.Status);
        }

        private static void ValidateThreadDetail(DetailTheadQuestionCourseModel model)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (model.IdTheadQuestionCourse <= 0 || model.IdStudent <= 0)
            {
                throw new ProjectBadRequestException("Thread and student are required.");
            }

            ValidateText(model.Comment, nameof(model.Comment), 1000);
            ValidateStatus(model.Status);
        }

        private static void ValidateComment(CourseEvaluationModel model)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (model.IdCourse <= 0 || model.IdStudent <= 0)
            {
                throw new ProjectBadRequestException("Course and student are required.");
            }

            if (model.VoteStartNumber < 1 || model.VoteStartNumber > 5)
            {
                throw new ProjectBadRequestException("Vote must be between 1 and 5.");
            }

            ValidateText(model.CommentEvaluation, nameof(model.CommentEvaluation), 1000);
            ValidateStatus(model.Status);
        }

        public async Task<bool> LikeForCommentCourse(long id)
        {
            var getData = await _courseEvaluationRepository.GetByIdAsync(id);
            if (getData == null)
            {
                return false;
            }

            getData.Like += 1;
            return await _courseEvaluationRepository.UpdateAsync(getData);
        }

        public async Task<bool> DisLikeForCommentCourse(long id)
        {
            var getData = await _courseEvaluationRepository.GetByIdAsync(id);
            if (getData == null)
            {
                return false;
            }

            getData.DisLike += 1;
            return await _courseEvaluationRepository.UpdateAsync(getData);
        }
    }
}
