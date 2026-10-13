using AutoMapper;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.CommonFunction;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HDNXUdemyServices.Services
{
    public class StudentServices : IStudentServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentPromotionRepository _studentPromotionRepository;
        private readonly IStudentProcessRepository _studentProcessRepository;
        private readonly IBookmarkCourseRepository _bookmarkCourseRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IRPPurcharseCourseDetailsRepository _purcharseCourseDetailsRepository;
        private readonly IRCourseEvaluationRepository _courseEvaluationRepository;
        private readonly IContentCourseRepository _contentCourseRepository;
        private readonly IContentCourseDetailRepository _contentCourseDetailRepository;
        private readonly IPurcharseCourseRepository _pucharseCourseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentServices(IUserRepository userRepository, IMapper mapper, IStudentPromotionRepository studentPromotionRepository,
            IStudentProcessRepository studentProcessRepository, IBookmarkCourseRepository bookmarkCourseRepository, ICategoryRepository categoryRepository,
            ICourseRepository courseRepository, IRPPurcharseCourseDetailsRepository purcharseCourseDetailsRepository, IRCourseEvaluationRepository courseEvaluationRepository,
            IContentCourseRepository contentCourseRepository, IContentCourseDetailRepository contentCourseDetailRepository, IPurcharseCourseRepository pucharseCourseRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository ?? throw new ProjectException(nameof(_userRepository));
            _mapper = mapper ?? throw new ProjectException(nameof(_mapper));
            _studentPromotionRepository = studentPromotionRepository ?? throw new ProjectException(nameof(_studentPromotionRepository));
            _studentProcessRepository = studentProcessRepository ?? throw new ProjectException(nameof(_studentProcessRepository));
            _bookmarkCourseRepository = bookmarkCourseRepository ?? throw new ProjectException(nameof(_bookmarkCourseRepository));
            _categoryRepository = categoryRepository ?? throw new ProjectException(nameof(_categoryRepository));
            _courseRepository = courseRepository ?? throw new ProjectException(nameof(_courseRepository));
            _purcharseCourseDetailsRepository = purcharseCourseDetailsRepository ?? throw new ProjectException(nameof(_purcharseCourseDetailsRepository));
            _courseEvaluationRepository = courseEvaluationRepository ?? throw new ProjectException(nameof(_courseEvaluationRepository));
            _contentCourseRepository = contentCourseRepository ?? throw new ProjectException(nameof(_contentCourseRepository));
            _contentCourseDetailRepository = contentCourseDetailRepository ?? throw new ProjectException(nameof(_contentCourseDetailRepository));
            _pucharseCourseRepository = pucharseCourseRepository ?? throw new ProjectException(nameof(_pucharseCourseRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ProjectException(nameof(_httpContextAccessor));
        }

        public async Task<bool> CreateStudent(UserModel model)
        {
            bool isCheckEmailExit = string.IsNullOrEmpty((await _userRepository.GetObjectAsync(x => x.Email == model.Email))?.Email);
            if (isCheckEmailExit)
            {
                model.IsRequestTeacher = false;
                model.RoleId = (int)ERoles.Student;
                model.Password = PasswordHasher.Hash(Generator.GeneratePassword(10));
                var dataInsert = _mapper.Map<UserEntities>(model);
                return await _userRepository.AddAsync(dataInsert);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusStudent(long id, UserModel model)
        {
            var getData = await _userRepository.GetByIdAsync(id) ?? new UserEntities();
            getData.Status = model.Status;
            return await _userRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateBecomeToTeacher(long id)
        {
            var getData = await _userRepository.GetByIdAsync(id) ?? new UserEntities();
            getData.IsRequestTeacher = true;
            getData.RoleId = (int)ERoles.Teacher;
            return await _userRepository.UpdateAsync(getData);
        }

        public async Task<bool> UpdateStudent(long id, UserModel model)
        {
            var getData = await _userRepository.GetByIdAsync(id);
            if (getData == null)
            {
                throw new ProjectNotFoundException("Student not found.");
            }

            ValidateProfile(model);
            var normalizedEmail = model.Email!.Trim().ToLowerInvariant();
            var duplicateEmail = await _userRepository.GetObjectAsync(x => x.Id != id && x.Email != null && x.Email.ToLower() == normalizedEmail);
            if (duplicateEmail != null)
            {
                throw new ProjectConflictException("Email already exists.");
            }

            getData.Status = model.Status;
            getData.Email = normalizedEmail;
            getData.PictureUrl = model.PictureUrl;
            getData.Phone = model.Phone?.Trim();
            getData.General = model.General;
            getData.PublicId = model.PublicId;
            getData.RoleId = model.RoleId;
            getData.Name = model.Name?.Trim();
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                ValidatePassword(model.Password);
                getData.Password = PasswordHasher.Hash(model.Password);
            }
            return await _userRepository.UpdateAsync(getData);
        }

        private static void ValidateProfile(UserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ProjectBadRequestException("Name is required.");
            }

            if (model.Name.Trim().Length > 100)
            {
                throw new ProjectBadRequestException("Name must be 100 characters or fewer.");
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ProjectBadRequestException("Email is required.");
            }

            try
            {
                _ = new MailAddress(model.Email.Trim());
            }
            catch
            {
                throw new ProjectBadRequestException("Invalid email format.");
            }

            if (!string.IsNullOrWhiteSpace(model.Phone) && model.Phone.Trim().Length > 20)
            {
                throw new ProjectBadRequestException("Phone must be 20 characters or fewer.");
            }
        }

        private static void ValidatePassword(string password)
        {
            if (password.Length < 8 ||
                !Regex.IsMatch(password, "[A-Z]") ||
                !Regex.IsMatch(password, "[a-z]") ||
                !Regex.IsMatch(password, "[0-9]"))
            {
                throw new ProjectBadRequestException("Password must be at least 8 characters and include uppercase, lowercase, and number.");
            }
        }

        public async Task<List<UserModel>> GetListStudents()
        {
            var getData = await _userRepository.GetAsync(x => x.RoleId == (int)ERoles.Student);
            return _mapper.Map<List<UserModel>>(getData);
        }

        public async Task<PagedResult<UserModel>> GetListStudentsPaged(int pageIndex, int pageSize)
        {
            var getData = await _userRepository.GetAsync(x => x.RoleId == (int)ERoles.Student);
            var resultMapping = _mapper.Map<List<UserModel>>(getData);
            return resultMapping.GetPagingPaged(pageIndex, pageSize, null);
        }

        public async Task<List<UserModel>> GetListUserManager()
        {
            var getData = await _userRepository.GetAsync(x => x.RoleId == (int)ERoles.User || x.RoleId == (int)ERoles.Admin);
            return _mapper.Map<List<UserModel>>(getData);
        }

        public async Task<PagedResult<UserModel>> GetListUserManagerPaged(int pageIndex, int pageSize)
        {
            var getData = await _userRepository.GetAsync(x => x.RoleId == (int)ERoles.User || x.RoleId == (int)ERoles.Admin);
            var resultMapping = _mapper.Map<List<UserModel>>(getData);
            return resultMapping.GetPagingPaged(pageIndex, pageSize, null);
        }

        public async Task<List<UserModel>> GetListTeachers()
        {
            var getData = await _userRepository.GetAsync(x => x.RoleId == (int)ERoles.Teacher);
            return _mapper.Map<List<UserModel>>(getData);
        }

        public async Task<UserModel> GetStudent(long id)
        {
            var getData = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserModel>(getData);
        }

        public async Task<bool> CreateStudentPromotions(StudentPromotionModel model)
        {
            ValidateStudentPromotion(model);
            var dataInsert = _mapper.Map<StudentPromotionEntities>(model);
            return await _studentPromotionRepository.AddAsync(dataInsert);
        }

        public async Task<bool> UpdateStatusStudentPromotions(long id, StudentPromotionModel model)
        {
            ValidateId(id);
            ValidateStatus(model?.Status ?? -1);
            var getData = await _studentPromotionRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Student promotion not found.");
            getData.Status = model.Status;
            return await _studentPromotionRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateInformationStudentPromotions(long id, StudentPromotionModel model)
        {
            ValidateId(id);
            ValidateStudentPromotion(model);
            var getData = await _studentPromotionRepository.GetByIdAsync(id)
                ?? throw new ProjectNotFoundException("Student promotion not found.");
            getData.Status = model.Status;
            getData.IdCourse = model.IdCourse;
            getData.IdStudent = model.IdStudent;
            getData.IdPromotion = model.IdPromotion;
            getData.CodePromotion = model.CodePromotion;
            getData.ValuePromotion = model.ValuePromotion;
            return await _studentPromotionRepository.UpdateAsync(getData);
        }

        public async Task<List<StudentPromotionModel>> GetListStudentPromotions()
        {
            var getData = await _studentPromotionRepository.GetAllAsync();
            return _mapper.Map<List<StudentPromotionModel>>(getData);
        }

        public async Task<StudentPromotionModel> GetStudentPromotions(long id)
        {
            var getData = await _studentPromotionRepository.GetByIdAsync(id);
            return _mapper.Map<StudentPromotionModel>(getData);
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

        private static void ValidateStudentPromotion(StudentPromotionModel model)
        {
            if (model == null)
            {
                throw new ProjectBadRequestException("Payload is required.");
            }

            if (model.IdCourse <= 0 || model.IdStudent <= 0 || model.IdPromotion <= 0)
            {
                throw new ProjectBadRequestException("Course, student and promotion are required.");
            }

            if (string.IsNullOrWhiteSpace(model.CodePromotion))
            {
                throw new ProjectBadRequestException("Promotion code is required.");
            }

            if (model.CodePromotion.Length > 100 || model.CodePromotion.Contains("<script", StringComparison.OrdinalIgnoreCase))
            {
                throw new ProjectBadRequestException("Promotion code is invalid.");
            }

            if (model.ValuePromotion <= 0)
            {
                throw new ProjectBadRequestException("Promotion value is invalid.");
            }

            ValidateStatus(model.Status);
        }

        public async Task<bool> CreateStudentProcess(StudentProcessModel model)
        {
            await ValidateStudentProcess(model);
            var dataInsert = _mapper.Map<StudentProcessEntities>(model);
            return await _studentProcessRepository.AddAsync(dataInsert);
        }

        public async Task<bool> UpdateStatusStudentProcess(long id, StudentProcessModel model)
        {
            var getData = await GetOwnedStudentProcess(id);
            if (!Enum.IsDefined(typeof(EStatus), model.Status))
            {
                throw new ProjectBadRequestException("Status is invalid.");
            }

            getData.Status = model.Status;
            return await _studentProcessRepository.UpdateStatusAsync(getData);
        }

        public async Task<bool> UpdateInformationStudentProcess(long id, StudentProcessModel model)
        {
            var getData = await GetOwnedStudentProcess(id);
            await ValidateStudentProcess(model);
            getData.Status = model.Status;
            getData.IdCourse = model.IdCourse;
            getData.NumberContentOfCourse = model.NumberContentOfCourse;
            getData.LastContentOfContent = model.LastContentOfContent;
            return await _studentProcessRepository.UpdateAsync(getData);
        }

        public async Task<List<StudentProcessModel>> GetListStudentProcess()
        {
            var getData = IsCurrentUserAdmin()
                ? await _studentProcessRepository.GetAllAsync()
                : await _studentProcessRepository.GetAsync(x => x.CreateBy == GetCurrentUserId());
            return _mapper.Map<List<StudentProcessModel>>(getData);
        }

        public async Task<StudentProcessModel> GetStudentProcess(long id)
        {
            var getData = await GetOwnedStudentProcess(id);
            return _mapper.Map<StudentProcessModel>(getData);
        }

        private async Task ValidateStudentProcess(StudentProcessModel model)
        {
            if (model.IdCourse <= 0)
            {
                throw new ProjectBadRequestException("Course is required.");
            }

            var course = await _courseRepository.GetObjectAsync(x => x.Id == model.IdCourse && x.Status == (int)EStatus.Active);
            if (course == null)
            {
                throw new ProjectNotFoundException("Course not found.");
            }

            if (model.NumberContentOfCourse < 0)
            {
                throw new ProjectBadRequestException("NumberContentOfCourse cannot be negative.");
            }

            if (model.LastContentOfContent < 0)
            {
                throw new ProjectBadRequestException("LastContentOfContent cannot be negative.");
            }

            if (!Enum.IsDefined(typeof(EStatus), model.Status))
            {
                throw new ProjectBadRequestException("Status is invalid.");
            }

            var courseContents = await _contentCourseRepository.GetAsync(x => x.IdCourse == model.IdCourse && x.Status == (int)EStatus.Active);
            var contentIds = courseContents.Select(x => x.Id).ToList();
            var courseContentDetails = contentIds.Count == 0
                ? new List<DNXUdemyData.Entities.ContentCourseDetailEntities>()
                : (await _contentCourseDetailRepository.GetAsync(x => contentIds.Contains(x.IdContent) && x.Status == (int)EStatus.Active)).ToList();
            var contentCount = courseContentDetails.Count;

            if (contentCount == 0 && (model.NumberContentOfCourse != 0 || model.LastContentOfContent != 0))
            {
                throw new ProjectBadRequestException("Course content range is invalid.");
            }

            if (contentCount > 0 && (model.NumberContentOfCourse > contentCount || model.LastContentOfContent > contentCount))
            {
                throw new ProjectBadRequestException("Course content range is invalid.");
            }

            if (!course.IsFree && !IsCurrentUserAdmin() && !await IsCurrentUserEnrolled(model.IdCourse))
            {
                throw new ProjectNotFoundException("Course not found.");
            }
        }

        private async Task<bool> IsCurrentUserEnrolled(long courseId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return false;
            }

            var purchaseDetails = await _purcharseCourseDetailsRepository.GetAsync(x => x.IdCourse == courseId && x.IdStudent == currentUserId);
            foreach (var purchaseDetail in purchaseDetails)
            {
                var purchaseOrder = await _pucharseCourseRepository.GetByIdAsync(purchaseDetail.IdPurchaseOrder);
                if (purchaseOrder?.PurcharseStatus == (int)ETypeOfStatusOrder.Payment && purchaseOrder.Status == (int)EStatus.Active)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<StudentProcessEntities> GetOwnedStudentProcess(long id)
        {
            var getData = await _studentProcessRepository.GetByIdAsync(id);
            if (getData == null)
            {
                throw new ProjectNotFoundException("Student process not found.");
            }

            if (!IsCurrentUserAdmin() && getData.CreateBy != GetCurrentUserId())
            {
                throw new ProjectNotFoundException("Student process not found.");
            }

            return getData;
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

        public async Task<bool> CreateStudentBookmarkCourse(BookmarkCourseModel model)
        {
            var dataInsert = _mapper.Map<BookmarkCourseEntities>(model);
            var isCheckData = await _bookmarkCourseRepository.GetObjectAsync(x => x.IdCourse == model.IdCourse && x.IdStudent == model.IdStudent);
            if (isCheckData == null)
            {
                return await _bookmarkCourseRepository.AddAsync(dataInsert);
            }
            else
            {
                return await _bookmarkCourseRepository.DeleteByKey(isCheckData.Id);
            }
        }

        public async Task<bool> UpdateStatusStudentBookmarkCourse(long id, BookmarkCourseModel model)
        {
            var getData = await _bookmarkCourseRepository.GetByIdAsync(id) ?? new BookmarkCourseEntities();
            getData.Status = model.Status;
            return await _bookmarkCourseRepository.UpdateStatusAsync(getData);
        }

        public async Task<List<CourseModel>> GetListStudentBookmarkCourse(long idUser)
        {
            var getData = await _bookmarkCourseRepository.GetAsync(x => x.IdStudent == idUser);
            var listIdOfCoureBookmark = getData.DistinctBy(x => x.IdCourse).Select(x => x.IdCourse).ToList();

            var getDataOfCourse = await _courseRepository.GetAsync(x => listIdOfCoureBookmark.Contains(x.Id));
            var getCategory = await _categoryRepository.GetAllAsync();
            var resultMapping = _mapper.Map<List<CourseModel>>(getDataOfCourse);

            foreach (var item in resultMapping)
            {
                var isPurchaseCourseDetails = await _purcharseCourseDetailsRepository.GetAsync(x => x.IdCourse == item.Id && x.IdStudent == idUser);
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
                item.UserName = (await _userRepository.GetByIdAsync(idUser))?.Name;
                item.ProcessCourseName = item.ProcessCourse.HasValue
                    ? ((ProcessVideo)item.ProcessCourse.Value).GetEnumDescription()
                    : null;
                item.IsPurchase = isPurchaseCourseDetails.Any();
                item.IsBookMark = true;
            }
            return resultMapping;
        }

        public async Task<bool> DeleteStudentBookmarkCourse(long id)
        {
            return await _bookmarkCourseRepository.DeleteByKey(id);
        }

        public async Task<List<string?>> GetListUserNameRegisterForCourse(long idCourse)
        {
            var getListDataStudentPurchase = await _purcharseCourseDetailsRepository.GetAsync(x => x.IdCourse == idCourse);
            List<long> listIdStudent = getListDataStudentPurchase.Select(x => x.IdStudent).ToList();
            return (await _userRepository.GetAsync(x => listIdStudent.Contains(x.Id))).Select(x => x.Name).ToList();
        }
    }
}
